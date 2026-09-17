import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/core/router/app_router.dart';
import 'package:sigis_mobile/core/theme/app_theme.dart';
import 'package:sigis_mobile/core/utils/inactivity_guard.dart';

/// Widget raiz do aplicativo SIGIS mobile.
///
/// Configura o `MaterialApp` com o tema do SUS (Material 3) e o
/// roteamento declarativo (`go_router`), incluindo o guard de
/// autenticacao.
class SigisApp extends ConsumerWidget {
  /// Cria o widget raiz do app.
  const SigisApp({super.key});

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final router = ref.watch(appRouterProvider);

    return MaterialApp.router(
      title: 'SIGIS',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.light,
      routerConfig: router,
      builder: (context, child) => InactivityGuard(child: child!),
    );
  }
}
