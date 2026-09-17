import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/core/errors/checkin_error.dart';
import 'package:sigis_mobile/core/providers/core_providers.dart';
import 'package:sigis_mobile/core/result/result.dart';
import 'package:sigis_mobile/data/models/enums.dart';
import 'package:sigis_mobile/features/auth/providers.dart';

/// Controller da tela de registro de sessao rapida (F06).
///
/// A escrita e sempre salva localmente primeiro (fila de
/// sincronizacao) — funciona 100% offline, ver `AttendanceRepository`
/// e `SyncService`.
class SessionController extends AutoDisposeAsyncNotifier<void> {
  @override
  Future<void> build() async {}

  /// Registra uma sessao rapida para a pessoa [patientId].
  Future<void> save({
    required String patientId,
    required SessionType sessionType,
    required String objective,
    String? notes,
    required bool usesMedication,
  }) async {
    final professional = await ref.read(currentUserProvider.future);
    if (professional == null) {
      state = AsyncError(CheckInError.attendanceNotFound, StackTrace.current);
      return;
    }

    state = const AsyncLoading();

    final result = await ref.read(attendanceRepositoryProvider).registerSession(
          patientId: patientId,
          unitId: professional.unitId,
          professionalId: professional.id,
          sessionType: sessionType,
          objective: objective,
          notes: notes,
          usesMedication: usesMedication,
        );

    state = switch (result) {
      Success<void>() => const AsyncData(null),
      Failure<void>(error: final error) => AsyncError(error, StackTrace.current),
    };
  }
}
