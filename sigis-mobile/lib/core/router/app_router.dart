import 'package:flutter/foundation.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:sigis_mobile/features/auth/presentation/login_page.dart';
import 'package:sigis_mobile/features/auth/providers.dart';
import 'package:sigis_mobile/features/checkin/presentation/checkin_page.dart';
import 'package:sigis_mobile/features/home/presentation/home_page.dart';
import 'package:sigis_mobile/features/patient/presentation/patient_detail_page.dart';
import 'package:sigis_mobile/features/search/presentation/search_page.dart';
import 'package:sigis_mobile/features/session/presentation/session_page.dart';
import 'package:sigis_mobile/features/sync/presentation/sync_page.dart';

/// Provider do roteador declarativo do app (`go_router`).
///
/// Aplica o guard de autenticacao: qualquer rota diferente de `/login`
/// redireciona para `/login` quando nao ha profissional autenticado, e
/// `/login` redireciona para `/home` quando ja autenticado.
final appRouterProvider = Provider<GoRouter>((ref) {
  final refreshNotifier = _AuthRefreshNotifier(ref);
  ref.onDispose(refreshNotifier.dispose);

  return GoRouter(
    initialLocation: '/login',
    refreshListenable: refreshNotifier,
    redirect: (context, state) {
      final isAuthenticated =
          ref.read(currentUserProvider).valueOrNull != null;
      final isLoggingIn = state.matchedLocation == '/login';

      if (!isAuthenticated && !isLoggingIn) {
        return '/login';
      }
      if (isAuthenticated && isLoggingIn) {
        return '/home';
      }
      return null;
    },
    routes: [
      GoRoute(
        path: '/login',
        builder: (context, state) => const LoginPage(),
      ),
      GoRoute(
        path: '/home',
        builder: (context, state) => const HomePage(),
      ),
      GoRoute(
        path: '/search',
        builder: (context, state) => const SearchPage(),
      ),
      GoRoute(
        path: '/patient/:id',
        builder: (context, state) => PatientDetailPage(
          patientId: state.pathParameters['id']!,
        ),
      ),
      GoRoute(
        path: '/checkin/:attendanceId',
        builder: (context, state) => CheckInPage(
          attendanceId: state.pathParameters['attendanceId']!,
          args: state.extra as CheckInArgs?,
        ),
      ),
      GoRoute(
        path: '/session/:patientId',
        builder: (context, state) => SessionPage(
          patientId: state.pathParameters['patientId']!,
        ),
      ),
      GoRoute(
        path: '/sync',
        builder: (context, state) => const SyncPage(),
      ),
    ],
  );
});

/// Notifica o `go_router` para reavaliar o [redirect] sempre que o
/// estado de autenticacao mudar.
class _AuthRefreshNotifier extends ChangeNotifier {
  _AuthRefreshNotifier(this._ref) {
    _subscription = _ref.listen<AsyncValue<dynamic>>(
      currentUserProvider,
      (previous, next) => notifyListeners(),
    );
  }

  final Ref _ref;
  late final ProviderSubscription<AsyncValue<dynamic>> _subscription;

  @override
  void dispose() {
    _subscription.close();
    super.dispose();
  }
}
