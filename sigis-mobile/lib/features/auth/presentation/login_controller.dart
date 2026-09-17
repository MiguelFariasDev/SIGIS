import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/core/errors/app_error.dart';
import 'package:sigis_mobile/core/providers/core_providers.dart';
import 'package:sigis_mobile/core/result/result.dart';
import 'package:sigis_mobile/data/models/professional.dart';
import 'package:sigis_mobile/features/auth/providers.dart';

/// Controller da tela de login (F01).
///
/// Expoe o estado da tentativa de login como `AsyncValue<void>`: sem
/// dados enquanto ocioso/carregando, ou erro contendo um [AppError]
/// quando as credenciais forem invalidas.
class LoginController extends AutoDisposeAsyncNotifier<void> {
  @override
  Future<void> build() async {}

  /// Tenta autenticar com [email] e [password].
  ///
  /// Em caso de sucesso, atualiza [currentUserProvider] com o
  /// profissional autenticado.
  Future<void> login({
    required String email,
    required String password,
    bool rememberMe = false,
  }) async {
    if (email.trim().isEmpty || password.trim().isEmpty) {
      state = AsyncError(AppError.unknown(), StackTrace.current);
      return;
    }

    state = const AsyncLoading();

    final result = await ref.read(authRepositoryProvider).login(
          email: email,
          password: password,
          rememberMe: rememberMe,
        );

    switch (result) {
      case Success<Professional>(value: final professional):
        ref.read(currentUserProvider.notifier).setUser(professional);
        state = const AsyncData(null);
      case Failure<Professional>(error: final error):
        state = AsyncError(error, StackTrace.current);
    }
  }
}
