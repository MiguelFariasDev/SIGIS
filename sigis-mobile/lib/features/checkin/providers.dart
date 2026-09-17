import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/features/checkin/presentation/checkin_controller.dart';

/// Provider do controller de check-in (F05).
final checkInControllerProvider =
    AsyncNotifierProvider.autoDispose<CheckInController, void>(
  CheckInController.new,
);
