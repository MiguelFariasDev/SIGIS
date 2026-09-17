import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:sigis_mobile/core/errors/app_error.dart';
import 'package:sigis_mobile/core/providers/core_providers.dart';
import 'package:sigis_mobile/core/theme/app_colors.dart';
import 'package:sigis_mobile/core/utils/formatters.dart';
import 'package:sigis_mobile/data/models/attendance.dart';
import 'package:sigis_mobile/data/models/enums.dart';
import 'package:sigis_mobile/data/models/patient.dart';
import 'package:sigis_mobile/features/checkin/presentation/checkin_page.dart';
import 'package:sigis_mobile/features/patient/providers.dart';
import 'package:sigis_mobile/shared/widgets/error_banner.dart';

/// Tela de resumo do paciente (F04).
///
/// Exibe identificacao basica, atendimentos recentes e acoes rapidas de
/// confirmar presenca e registrar sessao.
class PatientDetailPage extends ConsumerWidget {
  /// Cria a tela de resumo para a pessoa [patientId].
  const PatientDetailPage({super.key, required this.patientId});

  /// Identificador da pessoa exibida.
  final String patientId;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final patientState = ref.watch(patientDetailControllerProvider(patientId));
    final isOnline = ref.watch(connectivityStatusProvider).valueOrNull ?? true;

    return Scaffold(
      appBar: AppBar(
        title: Text(patientState.valueOrNull?.fullName ?? 'Paciente'),
      ),
      body: patientState.when(
        data: (patient) => _PatientDetailBody(patient: patient, isOnline: isOnline),
        loading: () => const Center(child: CircularProgressIndicator()),
        error: (error, stackTrace) => Center(
          child: ErrorBanner(
            message: error is AppError
                ? error.message
                : 'Nao foi possivel carregar o paciente.',
            onRetry: () => ref
                .read(patientDetailControllerProvider(patientId).notifier)
                .refresh(),
          ),
        ),
      ),
    );
  }
}

class _PatientDetailBody extends StatelessWidget {
  const _PatientDetailBody({required this.patient, required this.isOnline});

  final Patient patient;
  final bool isOnline;

  Attendance? get _nextScheduled => patient.recentAttendances
      ?.where((a) => a.status == AttendanceStatus.scheduled)
      .firstOrNull;

  bool get _hasRecentAbsence =>
      patient.recentAttendances?.any((a) => a.status == AttendanceStatus.absent) ??
      false;

  @override
  Widget build(BuildContext context) {
    final scheduled = _nextScheduled;

    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        if (!isOnline)
          const Padding(
            padding: EdgeInsets.only(bottom: 12),
            child: ErrorBanner(
              message: 'Dados podem estar desatualizados (offline).',
              isWarning: true,
            ),
          ),
        if (_hasRecentAbsence)
          const Padding(
            padding: EdgeInsets.only(bottom: 12),
            child: ErrorBanner(
              message: 'Paciente possui faltas recentes.',
              isWarning: true,
            ),
          ),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  patient.fullName,
                  style: const TextStyle(fontSize: 18, fontWeight: FontWeight.bold),
                ),
                const SizedBox(height: 4),
                Text(Formatters.ageLabel(patient.birthDate)),
                if (patient.cns != null) Text('CNS: ${patient.cns}'),
                if (patient.motherName != null)
                  Text('Mae: ${patient.motherName}'),
              ],
            ),
          ),
        ),
        const SizedBox(height: 16),
        Row(
          children: [
            Expanded(
              child: ElevatedButton.icon(
                onPressed: scheduled != null
                    ? () => context.push(
                          '/checkin/${scheduled.id}',
                          extra: CheckInArgs(
                            patientName: patient.fullName,
                            details: scheduled.sessionType.label,
                          ),
                        )
                    : null,
                icon: const Icon(Icons.check_circle_outline),
                label: const Text('Confirmar presenca'),
              ),
            ),
          ],
        ),
        const SizedBox(height: 8),
        Row(
          children: [
            Expanded(
              child: OutlinedButton.icon(
                onPressed: () => context.push('/session/${patient.id}'),
                icon: const Icon(Icons.note_add_outlined),
                label: const Text('Registrar sessao rapida'),
              ),
            ),
          ],
        ),
        const SizedBox(height: 24),
        Text(
          'Atendimentos recentes',
          style: Theme.of(context).textTheme.titleMedium,
        ),
        const SizedBox(height: 8),
        if (patient.recentAttendances == null || patient.recentAttendances!.isEmpty)
          Padding(
            padding: const EdgeInsets.symmetric(vertical: 16),
            child: Text(
              'Nenhum atendimento registrado.',
              style: TextStyle(color: AppColors.neutralMedium),
            ),
          )
        else
          ...patient.recentAttendances!.map(
            (attendance) => Card(
              child: ListTile(
                title: Text(attendance.sessionType.label),
                subtitle: Text(Formatters.dateTime(attendance.dateTime)),
                trailing: Text(_statusLabel(attendance.status)),
              ),
            ),
          ),
      ],
    );
  }

  String _statusLabel(AttendanceStatus status) => switch (status) {
        AttendanceStatus.scheduled => 'Agendado',
        AttendanceStatus.attended => 'Compareceu',
        AttendanceStatus.absent => 'Faltou',
      };
}

extension _FirstOrNull<T> on Iterable<T> {
  T? get firstOrNull => isEmpty ? null : first;
}
