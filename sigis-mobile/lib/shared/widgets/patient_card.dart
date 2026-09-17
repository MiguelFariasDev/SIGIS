import 'package:flutter/material.dart';
import 'package:sigis_mobile/core/theme/app_colors.dart';
import 'package:sigis_mobile/core/utils/formatters.dart';
import 'package:sigis_mobile/data/models/enums.dart';
import 'package:sigis_mobile/data/models/queue_entry.dart';
import 'package:sigis_mobile/shared/widgets/priority_badge.dart';
import 'package:sigis_mobile/shared/widgets/status_badge.dart';

/// Cartao de um item da fila do dia, usado na [HomePage] (F02).
///
/// Exibe nome, idade, prioridade, status, tempo de espera e, quando o
/// item esta [QueueStatus.waiting], um botao de acao rapida de
/// check-in.
class PatientCard extends StatelessWidget {
  /// Cria o cartao para o item de fila [entry].
  const PatientCard({
    super.key,
    required this.entry,
    this.onTap,
    this.onCheckIn,
  });

  /// Item de fila exibido.
  final QueueEntry entry;

  /// Acao ao tocar no cartao (abre o resumo do paciente).
  final VoidCallback? onTap;

  /// Acao rapida de check-in, exibida apenas quando o status for
  /// [QueueStatus.waiting].
  final VoidCallback? onCheckIn;

  @override
  Widget build(BuildContext context) {
    final waitingTime = DateTime.now().difference(entry.enteredAt);

    return Card(
      margin: const EdgeInsets.symmetric(horizontal: 16, vertical: 6),
      child: InkWell(
        borderRadius: BorderRadius.circular(16),
        onTap: onTap,
        child: Padding(
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                children: [
                  Expanded(
                    child: Text(
                      entry.patientName,
                      style: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.w700,
                      ),
                      overflow: TextOverflow.ellipsis,
                    ),
                  ),
                  PriorityBadge(priority: entry.priority),
                ],
              ),
              const SizedBox(height: 4),
              Text(
                Formatters.ageLabel(entry.patientBirthDate),
                style: TextStyle(color: AppColors.neutralMedium),
              ),
              const SizedBox(height: 8),
              Row(
                children: [
                  StatusBadge(status: entry.status),
                  const SizedBox(width: 8),
                  Icon(
                    Icons.schedule,
                    size: 14,
                    color: AppColors.neutralMedium,
                  ),
                  const SizedBox(width: 4),
                  Text(
                    'Espera: ${Formatters.waitingTime(waitingTime)}',
                    style: TextStyle(
                      fontSize: 12,
                      color: AppColors.neutralMedium,
                    ),
                  ),
                ],
              ),
              if (entry.status == QueueStatus.waiting && onCheckIn != null) ...[
                const SizedBox(height: 12),
                SizedBox(
                  width: double.infinity,
                  child: OutlinedButton(
                    onPressed: onCheckIn,
                    child: const Text('Check-in'),
                  ),
                ),
              ],
            ],
          ),
        ),
      ),
    );
  }
}
