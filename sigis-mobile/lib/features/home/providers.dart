import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/data/models/queue_entry.dart';
import 'package:sigis_mobile/features/home/presentation/home_controller.dart';

/// Provider do controller da fila do dia (F02).
final homeControllerProvider =
    AsyncNotifierProvider.autoDispose<HomeController, List<QueueEntry>>(
  HomeController.new,
);
