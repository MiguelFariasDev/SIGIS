import 'dart:async';
import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:sigis_mobile/core/errors/checkin_error.dart';
import 'package:sigis_mobile/core/errors/patient_error.dart';
import 'package:sigis_mobile/core/result/result.dart';
import 'package:sigis_mobile/data/local/models/pending_operation.dart';
import 'package:sigis_mobile/data/models/attendance.dart';
import 'package:sigis_mobile/data/models/enums.dart';
import 'package:sigis_mobile/data/models/queue_entry.dart';
import 'package:sigis_mobile/data/remote/attendance_api.dart';
import 'package:sigis_mobile/sync/sync_queue.dart';
import 'package:sigis_mobile/sync/sync_service.dart';

/// Mapeia [AttendanceStatus] para o valor em portugues esperado pela
/// API (`comparecimento`).
String _attendanceStatusWireValue(AttendanceStatus status) => switch (status) {
      AttendanceStatus.scheduled => 'AGENDADO',
      AttendanceStatus.attended => 'COMPARECEU',
      AttendanceStatus.absent => 'FALTOU',
    };

/// Mapeia [SessionType] para o nome do enum C# esperado pela API
/// (`sessionType`, comparado com `Enum.TryParse` no backend).
String _sessionTypeWireValue(SessionType type) => switch (type) {
      SessionType.psychologicalAnamnesis => 'PsychologicalAnamnesis',
      SessionType.psychopedagogicalAnamnesis => 'PsychopedagogicalAnamnesis',
      SessionType.physicalEducationInstrument => 'PhysicalEducationInstrument',
      SessionType.nasfRecord => 'NasfRecord',
      SessionType.synthesis => 'Synthesis',
    };

/// Repositorio de fila e atendimentos.
///
/// Leituras (fila do dia, atendimentos recentes) vao direto a API.
/// Escritas (check-in e sessao rapida) SEMPRE passam primeiro pela fila
/// local de operacoes pendentes (`pending_operations`), garantindo
/// funcionamento 100% offline — ver [SyncService] para o fluxo de
/// sincronizacao.
class AttendanceRepository {
  /// Cria o repositorio com suas dependencias.
  AttendanceRepository({
    required AttendanceApi api,
    required SyncQueue syncQueue,
    required SyncService syncService,
  })  : _api = api,
        _syncQueue = syncQueue,
        _syncService = syncService;

  final AttendanceApi _api;
  final SyncQueue _syncQueue;
  final SyncService _syncService;

  /// Busca a fila de atendimento do dia para a unidade [unitId].
  Future<Result<List<QueueEntry>>> fetchQueueForUnit(String unitId) async {
    try {
      final rawResults = await _api.fetchQueue(unitId);
      final entries = rawResults
          .map((json) => QueueEntry.fromJson(json as Map<String, dynamic>))
          .toList();
      return Success(entries);
    } on DioException catch (exception) {
      return Failure(PatientError.fromDio(exception));
    }
  }

  /// Busca os atendimentos recentes de uma pessoa.
  Future<Result<List<Attendance>>> fetchRecentAttendances(
    String patientId,
  ) async {
    try {
      final rawResults = await _api.fetchAttendances(patientId);
      final attendances = rawResults
          .map((json) => Attendance.fromJson(json as Map<String, dynamic>))
          .toList();
      return Success(attendances);
    } on DioException catch (exception) {
      return Failure(PatientError.fromDio(exception));
    }
  }

  /// Confirma comparecimento ou falta de um atendimento agendado.
  ///
  /// Salva a operacao na fila local imediatamente (funciona offline) e
  /// dispara uma tentativa de sincronizacao em segundo plano quando
  /// houver conexao.
  Future<Result<void>> confirmAttendance({
    required String attendanceId,
    required AttendanceStatus status,
  }) async {
    if (status == AttendanceStatus.scheduled) {
      return Failure(CheckInError.missingSessionFields);
    }

    final payload = jsonEncode({
      'kind': 'checkin',
      'attendanceId': attendanceId,
      'comparecimento': _attendanceStatusWireValue(status),
    });

    await _syncQueue.enqueue(
      type: PendingOperationType.checkin,
      payload: payload,
    );

    unawaited(_syncService.syncNow());
    return const Success(null);
  }

  /// Registra uma sessao rapida para a pessoa [patientId].
  ///
  /// Salva a operacao na fila local imediatamente (funciona offline) e
  /// dispara uma tentativa de sincronizacao em segundo plano quando
  /// houver conexao.
  Future<Result<void>> registerSession({
    required String patientId,
    required String unitId,
    required String professionalId,
    required SessionType sessionType,
    required String objective,
    String? notes,
    required bool usesMedication,
  }) async {
    if (objective.trim().isEmpty) {
      return Failure(CheckInError.missingSessionFields);
    }

    final payload = jsonEncode({
      'kind': 'session',
      'personId': patientId,
      'unitId': unitId,
      'professionalId': professionalId,
      'sessionType': _sessionTypeWireValue(sessionType),
      'dateTime': DateTime.now().toIso8601String(),
      'formData': jsonEncode({
        'objetivo': objective,
        'observacoes': notes,
        'usoMedicacao': usesMedication,
      }),
    });

    await _syncQueue.enqueue(
      type: PendingOperationType.session,
      payload: payload,
    );

    unawaited(_syncService.syncNow());
    return const Success(null);
  }
}
