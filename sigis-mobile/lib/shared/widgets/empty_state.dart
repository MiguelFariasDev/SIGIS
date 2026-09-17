import 'package:flutter/material.dart';
import 'package:sigis_mobile/core/theme/app_colors.dart';

/// Estado vazio generico, usado quando uma lista nao possui itens.
class EmptyState extends StatelessWidget {
  /// Cria o estado vazio com [message] e um [icon] opcional.
  const EmptyState({
    super.key,
    required this.message,
    this.icon = Icons.inbox_outlined,
  });

  /// Mensagem exibida, em portugues.
  final String message;

  /// Icone exibido acima da mensagem.
  final IconData icon;

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Padding(
        padding: const EdgeInsets.all(24),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Icon(icon, size: 48, color: AppColors.neutralMedium),
            const SizedBox(height: 12),
            Text(
              message,
              textAlign: TextAlign.center,
              style: TextStyle(color: AppColors.neutralMedium),
            ),
          ],
        ),
      ),
    );
  }
}
