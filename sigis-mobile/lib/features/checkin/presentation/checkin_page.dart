import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:sigis_mobile/core/result/result.dart';
import 'package:sigis_mobile/core/theme/app_colors.dart';
import 'package:sigis_mobile/data/models/enums.dart';
import 'package:sigis_mobile/features/checkin/providers.dart';
import 'package:sigis_mobile/shared/extensions/context_extensions.dart';

/// Dados auxiliares exibidos na tela de check-in, passados via
/// navegacao (`extra` do `go_router`) para evitar uma requisicao
/// adicional apenas para exibicao.
class CheckInArgs {
  /// Cria os dados auxiliares de exibicao do check-in.
  const CheckInArgs({required this.patientName, this.details});

  /// Nome do paciente em destaque na tela.
  final String patientName;

  /// Detalhes do atendimento agendado (ex.: tipo de sessao/servico).
  final String? details;
}

/// Tela de confirmacao de comparecimento ou falta (F05).
///
/// Exige confirmacao dupla para "Faltou", evitando toque acidental.
class CheckInPage extends ConsumerWidget {
  /// Cria a tela de check-in para o atendimento [attendanceId].
  const CheckInPage({super.key, required this.attendanceId, this.args});

  /// Identificador do atendimento agendado.
  final String attendanceId;

  /// Dados auxiliares de exibicao (nome do paciente e detalhes).
  final CheckInArgs? args;

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    // Le apenas `isLoading` (nao usa `ref.listen`): o estado inicial de um
    // `AutoDisposeAsyncNotifier.build()` tambem passa por AsyncLoading ao
    // assentar, o que disparava falsamente a navegacao/snackbar de sucesso
    // assim que a tela abria. Em vez disso, o resultado de cada chamada e
    // tratado diretamente em [_confirm], como em `LoginPage._submit`.
    final isLoading = ref.watch(checkInControllerProvider).isLoading;

    return Scaffold(
      appBar: AppBar(title: const Text('Confirmar atendimento')),
      body: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.stretch,
          children: [
            Text(
              args?.patientName ?? 'Paciente',
              textAlign: TextAlign.center,
              style: context.textStyles.headlineSmall?.copyWith(
                fontWeight: FontWeight.bold,
              ),
            ),
            if (args?.details != null) ...[
              const SizedBox(height: 8),
              Text(
                args!.details!,
                textAlign: TextAlign.center,
                style: TextStyle(color: AppColors.neutralMedium),
              ),
            ],
            const SizedBox(height: 48),
            SizedBox(
              height: 64,
              child: ElevatedButton.icon(
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.secondary,
                  foregroundColor: Colors.white,
                ),
                onPressed: isLoading
                    ? null
                    : () => _confirm(context, ref, AttendanceStatus.attended),
                icon: const Icon(Icons.check_circle_outline),
                label: const Text('Compareceu'),
              ),
            ),
            const SizedBox(height: 16),
            SizedBox(
              height: 64,
              child: OutlinedButton.icon(
                style: OutlinedButton.styleFrom(
                  foregroundColor: AppColors.error,
                  side: const BorderSide(color: AppColors.error),
                ),
                onPressed: isLoading ? null : () => _confirmAbsence(context, ref),
                icon: const Icon(Icons.cancel_outlined),
                label: const Text('Faltou'),
              ),
            ),
          ],
        ),
      ),
    );
  }

  Future<void> _confirmAbsence(BuildContext context, WidgetRef ref) async {
    final confirmed = await showDialog<bool>(
      context: context,
      builder: (dialogContext) => AlertDialog(
        title: const Text('Confirmar falta'),
        content: Text(
          'Tem certeza que deseja registrar falta de '
          '${args?.patientName ?? 'paciente'}?',
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(dialogContext).pop(false),
            child: const Text('Cancelar'),
          ),
          FilledButton(
            style: FilledButton.styleFrom(backgroundColor: AppColors.error),
            onPressed: () => Navigator.of(dialogContext).pop(true),
            child: const Text('Confirmar falta'),
          ),
        ],
      ),
    );

    if (confirmed == true && context.mounted) {
      await _confirm(context, ref, AttendanceStatus.absent);
    }
  }

  /// Executa a confirmacao de [status] e trata o resultado: mostra a
  /// mensagem de sucesso e sai da tela, ou exibe o erro em um snackbar.
  Future<void> _confirm(
    BuildContext context,
    WidgetRef ref,
    AttendanceStatus status,
  ) async {
    final result = await ref
        .read(checkInControllerProvider.notifier)
        .confirm(attendanceId: attendanceId, status: status);

    if (!context.mounted) {
      return;
    }

    switch (result) {
      case Success<void>():
        context.showSnackBar('Presenca registrada');
        if (context.canPop()) {
          context.pop();
        } else {
          context.go('/home');
        }
      case Failure<void>(error: final error):
        context.showSnackBar(error.message, backgroundColor: AppColors.error);
    }
  }
}
