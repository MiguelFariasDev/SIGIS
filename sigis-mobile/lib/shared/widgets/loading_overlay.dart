import 'package:flutter/material.dart';

/// Indicador de carregamento centralizado, usado enquanto uma operacao
/// assincrona esta em andamento.
class LoadingOverlay extends StatelessWidget {
  /// Cria o indicador de carregamento.
  const LoadingOverlay({super.key, this.message});

  /// Mensagem opcional exibida abaixo do indicador (em portugues).
  final String? message;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Column(
        mainAxisSize: MainAxisSize.min,
        children: [
          const CircularProgressIndicator(),
          if (message != null) ...[
            const SizedBox(height: 16),
            Text(message!, style: Theme.of(context).textTheme.bodyMedium),
          ],
        ],
      ),
    );
  }
}
