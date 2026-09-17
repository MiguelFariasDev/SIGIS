import 'package:flutter/material.dart';
import 'package:sigis_mobile/core/theme/app_colors.dart';

/// Faixa de aviso/erro exibida no topo de uma tela, usada para mensagens
/// como "Modo offline" ou erros de carregamento.
class ErrorBanner extends StatelessWidget {
  /// Cria a faixa de aviso com [message] e, opcionalmente, [onRetry].
  const ErrorBanner({
    super.key,
    required this.message,
    this.onRetry,
    this.isWarning = false,
  });

  /// Mensagem exibida, em portugues.
  final String message;

  /// Acao de nova tentativa, exibida como botao "Tentar novamente".
  final VoidCallback? onRetry;

  /// Quando `true`, usa a cor de aviso (amarelo) em vez da cor de erro
  /// (vermelho) — util para avisos como "Modo offline".
  final bool isWarning;

  @override
  Widget build(BuildContext context) {
    final color = isWarning ? AppColors.warning : AppColors.error;
    final foreground = isWarning ? AppColors.neutralDark : Colors.white;

    return Container(
      width: double.infinity,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 10),
      color: isWarning ? color.withValues(alpha: 0.9) : color,
      child: Row(
        children: [
          Icon(
            isWarning ? Icons.wifi_off : Icons.error_outline,
            color: foreground,
            size: 20,
          ),
          const SizedBox(width: 8),
          Expanded(
            child: Text(
              message,
              style: TextStyle(color: foreground, fontWeight: FontWeight.w600),
            ),
          ),
          if (onRetry != null)
            TextButton(
              onPressed: onRetry,
              style: TextButton.styleFrom(foregroundColor: foreground),
              child: const Text('Tentar novamente'),
            ),
        ],
      ),
    );
  }
}
