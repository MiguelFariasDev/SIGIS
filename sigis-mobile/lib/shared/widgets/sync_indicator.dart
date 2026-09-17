import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:sigis_mobile/core/providers/core_providers.dart';
import 'package:sigis_mobile/core/theme/app_colors.dart';

/// Icone de sincronizacao exibido na AppBar, com badge indicando a
/// quantidade de operacoes pendentes (F07 — MSync).
///
/// Ao ser tocado, navega para a tela de sincronizacao (`/sync`).
class SyncIndicator extends ConsumerWidget {
  /// Cria o indicador de sincronizacao.
  const SyncIndicator({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final pendingCount = ref.watch(pendingSyncCountProvider).valueOrNull ?? 0;

    return IconButton(
      tooltip: 'Sincronizacao',
      onPressed: () => context.push('/sync'),
      icon: Badge(
        isLabelVisible: pendingCount > 0,
        label: Text('$pendingCount'),
        backgroundColor: AppColors.warning,
        textColor: AppColors.neutralDark,
        child: const Icon(Icons.sync),
      ),
    );
  }
}
