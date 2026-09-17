import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:sigis_mobile/core/network/connectivity_service.dart';
import 'package:sigis_mobile/data/local/dao/pending_operation_dao.dart';
import 'package:sigis_mobile/data/local/dao/sync_metadata_dao.dart';
import 'package:sigis_mobile/data/local/database.dart';
import 'package:sigis_mobile/data/local/models/pending_operation.dart';
import 'package:sigis_mobile/data/remote/attendance_api.dart';
import 'package:sigis_mobile/sync/sync_queue.dart';
import 'package:sigis_mobile/sync/sync_service.dart';
import 'package:sqflite_common_ffi/sqflite_ffi.dart';

class _FakeConnectivityService extends ConnectivityService {
  _FakeConnectivityService({this.online = true});

  bool online;

  @override
  Future<bool> isOnline() async => online;

  @override
  Stream<bool> get onStatusChange => const Stream.empty();
}

class _FakeAttendanceApi extends AttendanceApi {
  _FakeAttendanceApi({this.updateBehavior}) : super(Dio());

  Future<Map<String, dynamic>> Function(String id, Map<String, dynamic> body)?
      updateBehavior;

  @override
  Future<Map<String, dynamic>> update(
    String id,
    Map<String, dynamic> payload,
  ) {
    return updateBehavior?.call(id, payload) ??
        Future.value({'id': 'server-$id'});
  }

  @override
  Future<Map<String, dynamic>> create(Map<String, dynamic> payload) {
    return Future.value({'id': 'server-new'});
  }
}

void main() {
  late AppDatabase database;
  late SyncQueue syncQueue;
  late SyncMetadataDao syncMetadataDao;

  setUpAll(() {
    sqfliteFfiInit();
  });

  setUp(() async {
    database = AppDatabase(
      databaseFactory: databaseFactoryFfi,
      databasePath: inMemoryDatabasePath,
    );
    // sqflite mantem um cache de conexoes por path; como todos os testes
    // usam o mesmo `inMemoryDatabasePath`, a conexao (e os dados) e
    // reaproveitada entre testes. Limpa explicitamente para isolar cada
    // teste.
    await database.wipeAllData();
    syncQueue = SyncQueue(PendingOperationDao(database));
    syncMetadataDao = SyncMetadataDao(database);
  });

  Future<PendingOperation> enqueueCheckin() {
    return syncQueue.enqueue(
      type: PendingOperationType.checkin,
      payload: jsonEncode({
        'kind': 'checkin',
        'attendanceId': 'attendance-1',
        'comparecimento': 'COMPARECEU',
      }),
    );
  }

  test('sincroniza uma operacao pendente com sucesso quando online', () async {
    await enqueueCheckin();

    final service = SyncService(
      api: _FakeAttendanceApi(),
      syncQueue: syncQueue,
      connectivity: _FakeConnectivityService(online: true),
      syncMetadataDao: syncMetadataDao,
    );

    await service.syncNow();

    final operations = await syncQueue.all();
    expect(operations, hasLength(1));
    expect(operations.first.status, PendingOperationStatus.synced);
    expect(operations.first.serverId, isNotNull);

    service.dispose();
  });

  test('nao tenta sincronizar quando offline', () async {
    await enqueueCheckin();

    final service = SyncService(
      api: _FakeAttendanceApi(),
      syncQueue: syncQueue,
      connectivity: _FakeConnectivityService(online: false),
      syncMetadataDao: syncMetadataDao,
    );

    await service.syncNow();

    final operations = await syncQueue.all();
    expect(operations.first.status, PendingOperationStatus.pending);

    service.dispose();
  });

  test('marca operacao como conflito (409) para revisao humana, nunca silenciosa', () async {
    await enqueueCheckin();

    final service = SyncService(
      api: _FakeAttendanceApi(
        updateBehavior: (id, body) => Future.error(
          DioException(
            requestOptions: RequestOptions(path: '/api/atendimentos/$id'),
            response: Response(
              requestOptions: RequestOptions(path: '/api/atendimentos/$id'),
              statusCode: 409,
            ),
          ),
        ),
      ),
      syncQueue: syncQueue,
      connectivity: _FakeConnectivityService(online: true),
      syncMetadataDao: syncMetadataDao,
    );

    await service.syncNow();

    final operations = await syncQueue.all();
    expect(operations.first.status, PendingOperationStatus.conflict);
    expect(operations.first.errorMessage, isNotNull);

    service.dispose();
  });

  test('incrementa retryCount em falha de rede e marca failed apos o limite', () async {
    await enqueueCheckin();

    final service = SyncService(
      api: _FakeAttendanceApi(
        updateBehavior: (id, body) => Future.error(
          DioException(
            requestOptions: RequestOptions(path: '/api/atendimentos/$id'),
            type: DioExceptionType.connectionError,
          ),
        ),
      ),
      syncQueue: syncQueue,
      connectivity: _FakeConnectivityService(online: true),
      syncMetadataDao: syncMetadataDao,
    );

    for (var i = 0; i < kMaxSyncRetries; i++) {
      await service.syncNow();
    }

    final operations = await syncQueue.all();
    expect(operations.first.status, PendingOperationStatus.failed);
    expect(operations.first.retryCount, kMaxSyncRetries);

    service.dispose();
  });
}
