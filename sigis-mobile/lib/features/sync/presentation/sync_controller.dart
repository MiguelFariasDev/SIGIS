import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/core/providers/core_providers.dart';
import 'package:sigis_mobile/data/local/dao/sync_metadata_dao.dart';
import 'package:sigis_mobile/data/local/models/pending_operation.dart';

/// Controller da tela de sincronizacao (F07 — MSync).
///
/// Lista todas as operacoes pendentes/sincronizadas e permite forcar
/// uma nova tentativa de sincronizacao imediatamente.
class SyncController extends AutoDisposeAsyncNotifier<List<PendingOperation>> {
  @override
  Future<List<PendingOperation>> build() {
    return ref.read(syncQueueProvider).all();
  }

  /// Dispara uma sincronizacao imediata e recarrega a lista.
  Future<void> syncNow() async {
    await ref.read(syncServiceProvider).syncNow();
    await refresh();
  }

  /// Recarrega a lista de operacoes.
  Future<void> refresh() async {
    state = const AsyncLoading<List<PendingOperation>>().copyWithPrevious(
      state,
    );
    state = await AsyncValue.guard(() => ref.read(syncQueueProvider).all());
  }

  /// Reenvia uma operacao em conflito apos revisao humana.
  Future<void> retry(PendingOperation operation) async {
    await ref.read(syncQueueProvider).retry(operation);
    await syncNow();
  }

  /// Descarta uma operacao em conflito apos revisao humana.
  Future<void> discard(PendingOperation operation) async {
    await ref.read(syncQueueProvider).discard(operation.id);
    await refresh();
  }
}

/// Provider do instante da ultima sincronizacao bem-sucedida, exibido
/// na tela F07.
final lastSyncAtProvider = FutureProvider.autoDispose<DateTime?>((ref) async {
  final raw = await ref
      .watch(syncMetadataDaoProvider)
      .read(SyncMetadataDao.lastSyncKey);
  return raw == null ? null : DateTime.tryParse(raw);
});
