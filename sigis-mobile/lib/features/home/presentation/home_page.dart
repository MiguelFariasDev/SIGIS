import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:sigis_mobile/core/errors/app_error.dart';
import 'package:sigis_mobile/core/providers/core_providers.dart';
import 'package:sigis_mobile/data/models/enums.dart';
import 'package:sigis_mobile/features/auth/providers.dart';
import 'package:sigis_mobile/features/checkin/presentation/checkin_page.dart';
import 'package:sigis_mobile/features/home/providers.dart';
import 'package:sigis_mobile/shared/widgets/empty_state.dart';
import 'package:sigis_mobile/shared/widgets/error_banner.dart';
import 'package:sigis_mobile/shared/widgets/patient_card.dart';
import 'package:sigis_mobile/shared/widgets/sync_indicator.dart';

/// Tela inicial com a fila de atendimento do dia (F02).
///
/// Exibe a fila da unidade do profissional autenticado, com check-in
/// rapido para itens aguardando e um banner de aviso quando o
/// dispositivo esta offline.
class HomePage extends ConsumerWidget {
  /// Cria a tela da fila do dia.
  const HomePage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final queueState = ref.watch(homeControllerProvider);
    final professionalState = ref.watch(currentUserProvider);
    final isOnline = ref.watch(connectivityStatusProvider).valueOrNull ?? true;

    final professional = professionalState.valueOrNull;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Fila do Dia'),
        automaticallyImplyLeading: false,
        actions: [
          const SyncIndicator(),
          IconButton(
            tooltip: 'Sair',
            icon: const Icon(Icons.logout),
            onPressed: () => _logout(context, ref),
          ),
        ],
      ),
      body: Column(
        children: [
          if (!isOnline) const ErrorBanner(message: 'Modo offline', isWarning: true),
          Padding(
            padding: const EdgeInsets.fromLTRB(16, 12, 16, 4),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  professional != null
                      ? 'Ola, ${professional.name}'
                      : 'Ola',
                  style: Theme.of(context).textTheme.titleLarge,
                ),
                Text('Unidade: ${professional?.unitAcronym ?? '-'}'),
              ],
            ),
          ),
          Expanded(
            child: queueState.when(
              data: (entries) {
                if (entries.isEmpty) {
                  return const EmptyState(
                    message: 'Nenhum paciente na fila hoje.',
                    icon: Icons.groups_outlined,
                  );
                }
                return RefreshIndicator(
                  onRefresh: () =>
                      ref.read(homeControllerProvider.notifier).refresh(),
                  child: ListView.builder(
                    padding: const EdgeInsets.only(bottom: 96, top: 4),
                    itemCount: entries.length,
                    itemBuilder: (context, index) {
                      final entry = entries[index];
                      return PatientCard(
                        entry: entry,
                        onTap: () => context.push('/patient/${entry.patientId}'),
                        onCheckIn: entry.status == QueueStatus.waiting
                            ? () => context.push(
                                  '/checkin/${entry.id}',
                                  extra: CheckInArgs(
                                    patientName: entry.patientName,
                                    details: entry.specialty,
                                  ),
                                )
                            : null,
                      );
                    },
                  ),
                );
              },
              loading: () => const Center(child: CircularProgressIndicator()),
              error: (error, stackTrace) => Center(
                child: ErrorBanner(
                  message: error is AppError
                      ? error.message
                      : 'Nao foi possivel carregar a fila.',
                  onRetry: () =>
                      ref.read(homeControllerProvider.notifier).refresh(),
                ),
              ),
            ),
          ),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: () => context.push('/search'),
        icon: const Icon(Icons.search),
        label: const Text('Buscar paciente'),
      ),
    );
  }

  Future<void> _logout(BuildContext context, WidgetRef ref) async {
    await ref.read(authRepositoryProvider).logout();
    ref.read(currentUserProvider.notifier).clear();
    if (context.mounted) {
      context.go('/login');
    }
  }
}
