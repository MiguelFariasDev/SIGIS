import 'dart:convert';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/core/theme/app_colors.dart';
import 'package:sigis_mobile/core/utils/formatters.dart';
import 'package:sigis_mobile/data/local/models/pending_operation.dart';
import 'package:sigis_mobile/features/sync/presentation/sync_controller.dart';
import 'package:sigis_mobile/features/sync/providers.dart';
import 'package:sigis_mobile/shared/widgets/empty_state.dart';

/// Tela de indicador de sincronizacao (F07 — MSync).
///
/// Lista as operacoes pendentes de envio, permite forcar sincronizacao
/// imediata e exige revisao humana explicita para resolver conflitos
/// (nunca resolvidos silenciosamente).
class SyncPage extends ConsumerWidget {
  /// Cria a tela de sincronizacao.
  const SyncPage({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final operationsState = ref.watch(syncControllerProvider);
    final lastSyncState = ref.watch(lastSyncAtProvider);

    final pendingCount = operationsState.valueOrNull
            ?.where((op) => op.status != PendingOperationStatus.synced)
            .length ??
        0;

    return Scaffold(
      appBar: AppBar(title: const Text('Sincronizacao')),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.all(16),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  '$pendingCount pendente(s)'
                  '${lastSyncState.valueOrNull != null ? ' / ultima sync as '
                      '${Formatters.time(lastSyncState.valueOrNull!)}' : ''}',
                  style: Theme.of(context).textTheme.titleMedium,
                ),
                const SizedBox(height: 12),
                SizedBox(
                  width: double.infinity,
                  child: ElevatedButton.icon(
                    onPressed: () =>
                        ref.read(syncControllerProvider.notifier).syncNow(),
                    icon: const Icon(Icons.sync),
                    label: const Text('Sincronizar agora'),
                  ),
                ),
              ],
            ),
          ),
          const Divider(height: 1),
          Expanded(
            child: operationsState.when(
              data: (operations) {
                if (operations.isEmpty) {
                  return const EmptyState(
                    message: 'Nenhuma operacao pendente.',
                    icon: Icons.cloud_done_outlined,
                  );
                }
                return ListView.builder(
                  itemCount: operations.length,
                  itemBuilder: (context, index) => _OperationTile(
                    operation: operations[index],
                  ),
                );
              },
              loading: () => const Center(child: CircularProgressIndicator()),
              error: (error, stackTrace) => const EmptyState(
                message: 'Nao foi possivel carregar a fila de sincronizacao.',
                icon: Icons.error_outline,
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class _OperationTile extends ConsumerWidget {
  const _OperationTile({required this.operation});

  final PendingOperation operation;

  String get _typeLabel => switch (operation.operationType) {
        PendingOperationType.checkin => 'Check-in',
        PendingOperationType.session => 'Sessao rapida',
      };

  String get _statusLabel => switch (operation.status) {
        PendingOperationStatus.pending => 'Aguardando',
        PendingOperationStatus.syncing => 'Sincronizando',
        PendingOperationStatus.failed => 'Falhou',
        PendingOperationStatus.conflict => 'Conflito',
        PendingOperationStatus.synced => 'Sincronizado',
      };

  String get _patientLabel {
    try {
      final body = jsonDecode(operation.payload) as Map<String, dynamic>;
      return (body['pessoaNome'] as String?) ??
          (body['attendanceId'] as String?) ??
          'Paciente';
    } catch (_) {
      return 'Paciente';
    }
  }

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final isConflict = operation.status == PendingOperationStatus.conflict;

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      color: isConflict ? AppColors.error.withValues(alpha: 0.08) : null,
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                Expanded(
                  child: Text(
                    '$_typeLabel — $_patientLabel',
                    style: const TextStyle(fontWeight: FontWeight.w600),
                  ),
                ),
                Text(_statusLabel),
              ],
            ),
            const SizedBox(height: 4),
            Text(
              Formatters.dateTime(operation.createdAt),
              style: TextStyle(color: AppColors.neutralMedium, fontSize: 12),
            ),
            if (isConflict) ...[
              const SizedBox(height: 8),
              Text(
                operation.errorMessage ??
                    'Conflito detectado. Revise o registro manualmente.',
                style: const TextStyle(color: AppColors.error),
              ),
              const SizedBox(height: 8),
              Row(
                children: [
                  TextButton(
                    onPressed: () => ref
                        .read(syncControllerProvider.notifier)
                        .discard(operation),
                    child: const Text('Descartar'),
                  ),
                  const SizedBox(width: 8),
                  FilledButton(
                    onPressed: () => ref
                        .read(syncControllerProvider.notifier)
                        .retry(operation),
                    child: const Text('Resolver'),
                  ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }
}
