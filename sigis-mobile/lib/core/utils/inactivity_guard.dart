import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:sigis_mobile/core/config/app_config.dart';
import 'package:sigis_mobile/core/providers/core_providers.dart';
import 'package:sigis_mobile/core/router/app_router.dart';
import 'package:sigis_mobile/features/auth/providers.dart';

/// Encerra a sessao automaticamente apos
/// [AppConfig.sessionInactivityTimeout] sem nenhum toque na tela.
///
/// Envolve toda a arvore de rotas (autenticadas ou nao); qualquer toque
/// reinicia o cronometro. So efetua o logout quando ha, de fato, um
/// profissional autenticado no momento em que o tempo se esgota.
class InactivityGuard extends ConsumerStatefulWidget {
  /// Cria o guard de inatividade envolvendo [child].
  const InactivityGuard({super.key, required this.child});

  /// Conteudo atual do app, fornecido pelo `MaterialApp.router`.
  final Widget child;

  @override
  ConsumerState<InactivityGuard> createState() => _InactivityGuardState();
}

class _InactivityGuardState extends ConsumerState<InactivityGuard> {
  Timer? _timer;

  @override
  void initState() {
    super.initState();
    _resetTimer();
  }

  @override
  void dispose() {
    _timer?.cancel();
    super.dispose();
  }

  void _resetTimer() {
    _timer?.cancel();
    _timer = Timer(AppConfig.sessionInactivityTimeout, _onTimeout);
  }

  Future<void> _onTimeout() async {
    final isAuthenticated = ref.read(currentUserProvider).valueOrNull != null;
    if (!isAuthenticated) {
      _resetTimer();
      return;
    }

    await ref.read(authRepositoryProvider).logout();
    ref.read(currentUserProvider.notifier).clear();
    ref.read(appRouterProvider).go('/login');
  }

  @override
  Widget build(BuildContext context) {
    return Listener(
      behavior: HitTestBehavior.translucent,
      onPointerDown: (_) => _resetTimer(),
      child: widget.child,
    );
  }
}
