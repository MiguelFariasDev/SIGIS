import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/core/providers/core_providers.dart';
import 'package:sigis_mobile/data/models/professional.dart';
import 'package:sigis_mobile/features/auth/presentation/login_controller.dart';

/// Provider do profissional atualmente autenticado.
///
/// Usado pelo guard de rotas do `go_router` (redireciona para `/login`
/// quando `null`) e pela saudacao da [HomePage].
final currentUserProvider =
    AsyncNotifierProvider<CurrentUserNotifier, Professional?>(
  CurrentUserNotifier.new,
);

/// Mantem o estado do profissional autenticado, lido de
/// [AuthRepository.currentProfessional] na inicializacao.
class CurrentUserNotifier extends AsyncNotifier<Professional?> {
  @override
  Future<Professional?> build() {
    return ref.watch(authRepositoryProvider).currentProfessional();
  }

  /// Define o profissional autenticado apos um login bem-sucedido.
  void setUser(Professional professional) {
    state = AsyncData(professional);
  }

  /// Limpa o profissional autenticado apos logout.
  void clear() {
    state = const AsyncData(null);
  }
}

/// Provider do controller de login (F01).
final loginControllerProvider =
    AsyncNotifierProvider.autoDispose<LoginController, void>(
  LoginController.new,
);
