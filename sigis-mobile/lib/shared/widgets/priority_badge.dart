import 'package:flutter/material.dart';
import 'package:sigis_mobile/core/theme/app_colors.dart';
import 'package:sigis_mobile/data/models/enums.dart';

/// Badge colorido que exibe a prioridade de um item de fila.
class PriorityBadge extends StatelessWidget {
  /// Cria o badge para a [priority] informada.
  const PriorityBadge({super.key, required this.priority});

  /// Prioridade exibida.
  final QueuePriority priority;

  Color get _color => switch (priority) {
        QueuePriority.urgent => AppColors.error,
        QueuePriority.shortTerm => AppColors.warning,
        QueuePriority.waitingList => AppColors.neutralMedium,
      };

  @override
  Widget build(BuildContext context) {
    final isLight = priority == QueuePriority.shortTerm;
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(
        color: _color,
        borderRadius: BorderRadius.circular(999),
      ),
      child: Text(
        priority.label,
        style: TextStyle(
          color: isLight ? AppColors.neutralDark : Colors.white,
          fontSize: 12,
          fontWeight: FontWeight.w700,
        ),
      ),
    );
  }
}
