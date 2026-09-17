import 'dart:async';

import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/core/providers/core_providers.dart';
import 'package:sigis_mobile/core/result/result.dart';
import 'package:sigis_mobile/data/models/patient.dart';

/// Tempo de debounce da busca rapida (F03), para evitar uma requisicao
/// a cada tecla digitada.
const Duration kSearchDebounce = Duration(milliseconds: 300);

/// Controller da busca rapida de pacientes (F03).
///
/// Aplica debounce de 300ms antes de disparar a busca, combinando
/// resultados locais (cache) e remotos conforme a conectividade (ver
/// `PatientRepository.search`).
class SearchController extends AutoDisposeAsyncNotifier<List<Patient>> {
  Timer? _debounceTimer;
  String _lastTerm = '';

  @override
  Future<List<Patient>> build() async {
    ref.onDispose(() => _debounceTimer?.cancel());
    return const [];
  }

  /// Atualiza o termo de busca, disparando a consulta apos o debounce.
  void onTermChanged(String term) {
    _lastTerm = term;
    _debounceTimer?.cancel();

    if (term.trim().length < 2) {
      state = const AsyncData([]);
      return;
    }

    _debounceTimer = Timer(kSearchDebounce, () => _search(term));
  }

  Future<void> _search(String term) async {
    if (term != _lastTerm) {
      return;
    }

    state = const AsyncLoading<List<Patient>>().copyWithPrevious(
      AsyncData(state.valueOrNull ?? const []),
    );

    final result = await ref.read(patientRepositoryProvider).search(term);

    if (term != _lastTerm) {
      return;
    }

    state = switch (result) {
      Success<List<Patient>>(value: final patients) => AsyncData(patients),
      Failure<List<Patient>>(error: final error) =>
        AsyncError(error, StackTrace.current),
    };
  }
}
