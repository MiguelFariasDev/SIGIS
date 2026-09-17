import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/data/models/patient.dart';
import 'package:sigis_mobile/features/search/presentation/search_controller.dart';

/// Provider do controller de busca rapida (F03).
final searchControllerProvider =
    AsyncNotifierProvider.autoDispose<SearchController, List<Patient>>(
  SearchController.new,
);
