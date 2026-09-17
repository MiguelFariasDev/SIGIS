import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/core/providers/core_providers.dart';
import 'package:sigis_mobile/core/result/result.dart';
import 'package:sigis_mobile/data/models/queue_entry.dart';
import 'package:sigis_mobile/features/auth/providers.dart';

/// Controller da fila do dia (F02 — HomePage).
///
/// Carrega a fila de atendimento da unidade do profissional
/// autenticado. Em caso de falha (ex.: offline sem cache), propaga o
/// [AppError] como estado de erro do provider.
class HomeController extends AutoDisposeAsyncNotifier<List<QueueEntry>> {
  @override
  Future<List<QueueEntry>> build() => _load();

  Future<List<QueueEntry>> _load() async {
    final professional = await ref.watch(currentUserProvider.future);
    if (professional == null) {
      return const [];
    }

    final result = await ref
        .read(attendanceRepositoryProvider)
        .fetchQueueForUnit(professional.unitId);

    return switch (result) {
      Success<List<QueueEntry>>(value: final entries) => entries,
      Failure<List<QueueEntry>>(error: final error) => throw error,
    };
  }

  /// Recarrega a fila do dia (usado pelo pull-to-refresh).
  Future<void> refresh() async {
    state = const AsyncLoading<List<QueueEntry>>().copyWithPrevious(state);
    state = await AsyncValue.guard(_load);
  }
}
