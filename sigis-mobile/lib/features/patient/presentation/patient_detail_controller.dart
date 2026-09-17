import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/core/providers/core_providers.dart';
import 'package:sigis_mobile/core/result/result.dart';
import 'package:sigis_mobile/data/models/attendance.dart';
import 'package:sigis_mobile/data/models/patient.dart';

/// Numero maximo de atendimentos recentes exibidos no resumo do
/// paciente (F04).
const int kRecentAttendancesLimit = 5;

/// Controller do resumo do paciente (F04), parametrizado pelo
/// identificador da pessoa ([patientId]).
///
/// Carrega os dados de identificacao e os ultimos atendimentos da
/// pessoa. Segue a regra de LGPD de minimizacao: apenas os campos
/// exibidos na tela sao mantidos em memoria, nada e persistido em
/// disco alem do cache minimo de busca (`cached_patients`).
class PatientDetailController extends AutoDisposeFamilyAsyncNotifier<Patient, String> {
  @override
  Future<Patient> build(String patientId) async {
    final patientResult =
        await ref.read(patientRepositoryProvider).getById(patientId);

    final patient = switch (patientResult) {
      Success<Patient>(value: final value) => value,
      Failure<Patient>(error: final error) => throw error,
    };

    final attendancesResult = await ref
        .read(attendanceRepositoryProvider)
        .fetchRecentAttendances(patientId);

    final attendances = switch (attendancesResult) {
      Success<List<Attendance>>(value: final value) => value,
      Failure<List<Attendance>>() => const <Attendance>[],
    };

    final recent = attendances.take(kRecentAttendancesLimit).toList();
    return patient.copyWith(recentAttendances: recent);
  }

  /// Recarrega os dados do paciente.
  Future<void> refresh() async {
    state = const AsyncLoading<Patient>().copyWithPrevious(state);
    state = await AsyncValue.guard(() => build(arg));
  }
}
