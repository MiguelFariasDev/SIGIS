import 'package:flutter/material.dart';
import 'package:sigis_mobile/core/theme/app_colors.dart';
import 'package:sigis_mobile/data/models/enums.dart';

/// Badge colorido que exibe o status de um item de fila.
class StatusBadge extends StatelessWidget {
  /// Cria o badge para o [status] informado.
  const StatusBadge({super.key, required this.status});

  /// Status exibido.
  final QueueStatus status;

  Color get _color => switch (status) {
        QueueStatus.waiting => AppColors.neutralMedium,
        QueueStatus.inAttendance => AppColors.primary,
        QueueStatus.completed => AppColors.secondary,
        QueueStatus.absent => AppColors.error,
        QueueStatus.activeSearch => AppColors.warning,
      };

  @override
  Widget build(BuildContext context) {
    final isLight = status == QueueStatus.activeSearch;
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
      decoration: BoxDecoration(
        color: _color.withValues(alpha: 0.15),
        borderRadius: BorderRadius.circular(999),
        border: Border.all(color: _color),
      ),
      child: Text(
        status.label,
        style: TextStyle(
          color: isLight ? AppColors.neutralDark : _color,
          fontSize: 12,
          fontWeight: FontWeight.w700,
        ),
      ),
    );
  }
}
