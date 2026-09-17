import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/data/local/models/pending_operation.dart';
import 'package:sigis_mobile/features/sync/presentation/sync_controller.dart';

/// Provider do controller da tela de sincronizacao (F07).
final syncControllerProvider = AsyncNotifierProvider.autoDispose<
    SyncController, List<PendingOperation>>(
  SyncController.new,
);
