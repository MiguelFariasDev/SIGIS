import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/features/session/presentation/session_controller.dart';

/// Provider do controller de sessao rapida (F06).
final sessionControllerProvider =
    AsyncNotifierProvider.autoDispose<SessionController, void>(
  SessionController.new,
);
