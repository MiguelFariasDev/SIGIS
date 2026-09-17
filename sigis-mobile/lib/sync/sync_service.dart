import 'dart:async';
import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:sigis_mobile/core/errors/sync_error.dart';
import 'package:sigis_mobile/core/network/connectivity_service.dart';
import 'package:sigis_mobile/data/local/dao/sync_metadata_dao.dart';
import 'package:sigis_mobile/data/local/models/pending_operation.dart';
import 'package:sigis_mobile/data/remote/attendance_api.dart';
import 'package:sigis_mobile/sync/conflict_resolver.dart';
import 'package:sigis_mobile/sync/sync_queue.dart';

/// Numero maximo de tentativas antes de marcar uma operacao como
/// definitivamente falha (ainda assim continua na fila, visivel ao
/// usuario, mas deixa de ser reenviada automaticamente).
const int kMaxSyncRetries = 5;

/// Orquestra a sincronizacao das operacoes pendentes com o servidor.
///
/// Fluxo (offline-first): toda escrita e salva primeiro em
/// [SyncQueue]; este servico tenta enviar imediatamente quando ha
/// conexao e reenvia automaticamente quando a conectividade retorna.
/// Falhas incrementam `retryCount`; conflitos (HTTP 409) NUNCA sao
/// resolvidos silenciosamente — ficam marcados para revisao humana
/// (ver [ConflictResolver]).
class SyncService {
  /// Cria o servico com suas dependencias.
  SyncService({
    required AttendanceApi api,
    required SyncQueue syncQueue,
    required ConnectivityService connectivity,
    required SyncMetadataDao syncMetadataDao,
    ConflictResolver? conflictResolver,
  })  : _api = api,
        _syncQueue = syncQueue,
        _connectivity = connectivity,
        _syncMetadataDao = syncMetadataDao,
        _conflictResolver = conflictResolver ?? const ConflictResolver() {
    _connectivitySubscription = _connectivity.onStatusChange.listen((
      isOnline,
    ) {
      if (isOnline) {
        unawaited(syncNow());
      }
    });
  }

  final AttendanceApi _api;
  final SyncQueue _syncQueue;
  final ConnectivityService _connectivity;
  final SyncMetadataDao _syncMetadataDao;
  final ConflictResolver _conflictResolver;

  StreamSubscription<bool>? _connectivitySubscription;
  bool _isSyncing = false;

  final StreamController<int> _pendingCountController =
      StreamController<int>.broadcast();

  /// Emite a contagem de operacoes ainda nao sincronizadas sempre que
  /// ela muda — usado pelo badge do [SyncIndicator] no AppBar.
  Stream<int> get pendingCountStream => _pendingCountController.stream;

  /// Tenta sincronizar todas as operacoes pendentes agora.
  ///
  /// Nao faz nada se ja houver uma sincronizacao em andamento ou se o
  /// dispositivo estiver offline. Seguro de chamar repetidamente (ex.:
  /// apos cada escrita local e ao voltar a conexao).
  Future<void> syncNow() async {
    if (_isSyncing) {
      return;
    }

    final isOnline = await _connectivity.isOnline();
    if (!isOnline) {
      await _publishPendingCount();
      return;
    }

    _isSyncing = true;
    try {
      final operations = await _syncQueue.pendingOperations();
      for (final operation in operations) {
        await _syncOne(operation);
      }
      await _syncMetadataDao.write(
        SyncMetadataDao.lastSyncKey,
        DateTime.now().toIso8601String(),
      );
    } finally {
      _isSyncing = false;
      await _publishPendingCount();
    }
  }

  Future<void> _syncOne(PendingOperation operation) async {
    await _syncQueue.update(
      operation.copyWith(status: PendingOperationStatus.syncing),
    );

    try {
      final body = jsonDecode(operation.payload) as Map<String, dynamic>;
      final serverId = await _send(operation.operationType, body);

      await _syncQueue.update(
        operation.copyWith(
          status: PendingOperationStatus.synced,
          serverId: serverId,
          errorMessage: null,
        ),
      );
    } on DioException catch (exception) {
      if (exception.response?.statusCode == 409) {
        final reason = SyncError.conflict.message;
        await _syncQueue.update(
          _conflictResolver.markAsConflict(operation, reason),
        );
        return;
      }

      final nextRetryCount = operation.retryCount + 1;
      final exceededRetries = nextRetryCount >= kMaxSyncRetries;

      await _syncQueue.update(
        operation.copyWith(
          status: exceededRetries
              ? PendingOperationStatus.failed
              : PendingOperationStatus.pending,
          retryCount: nextRetryCount,
          errorMessage: SyncError.fromDio(exception).message,
        ),
      );
    }
  }

  Future<String?> _send(
    PendingOperationType type,
    Map<String, dynamic> body,
  ) async {
    switch (type) {
      case PendingOperationType.checkin:
        final attendanceId = body['attendanceId'] as String;
        final response = await _api.registerComparecimento(attendanceId, {
          'comparecimento': body['comparecimento'],
        });
        return response['id'] as String?;
      case PendingOperationType.session:
        final response = await _api.create({...body}..remove('kind'));
        return response['id'] as String?;
    }
  }

  Future<void> _publishPendingCount() async {
    final count = await _syncQueue.countPending();
    if (!_pendingCountController.isClosed) {
      _pendingCountController.add(count);
    }
  }

  /// Libera os recursos do servico (assinatura de conectividade e
  /// stream de contagem).
  void dispose() {
    _connectivitySubscription?.cancel();
    _pendingCountController.close();
  }
}
