import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:sigis_mobile/core/errors/app_error.dart';
import 'package:sigis_mobile/core/theme/app_colors.dart';
import 'package:sigis_mobile/features/auth/providers.dart';
import 'package:sigis_mobile/shared/extensions/context_extensions.dart';

/// Tela de login (F01).
///
/// Autentica contra o backend real (`POST /api/auth/login`) — ver
/// `AuthRepository.login`. Valida campos vazios localmente antes de
/// tentar autenticar.
class LoginPage extends ConsumerStatefulWidget {
  /// Cria a tela de login.
  const LoginPage({super.key});

  @override
  ConsumerState<LoginPage> createState() => _LoginPageState();
}

class _LoginPageState extends ConsumerState<LoginPage> {
  final _formKey = GlobalKey<FormState>();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();

  bool _obscurePassword = true;
  bool _rememberMe = false;

  @override
  void dispose() {
    _emailController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) {
      return;
    }

    await ref.read(loginControllerProvider.notifier).login(
          email: _emailController.text,
          password: _passwordController.text,
          rememberMe: _rememberMe,
        );

    if (!mounted) {
      return;
    }

    final state = ref.read(loginControllerProvider);
    if (state.hasError) {
      final error = state.error;
      final message = error is AppError ? error.message : '$error';
      context.showSnackBar(message, backgroundColor: AppColors.error);
      return;
    }

    context.go('/home');
  }

  @override
  Widget build(BuildContext context) {
    final loginState = ref.watch(loginControllerProvider);
    final isLoading = loginState.isLoading;

    return Scaffold(
      body: SafeArea(
        child: Center(
          child: SingleChildScrollView(
            padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 32),
            child: Form(
              key: _formKey,
              child: Column(
                mainAxisSize: MainAxisSize.min,
                crossAxisAlignment: CrossAxisAlignment.stretch,
                children: [
                  const _LogoPlaceholder(),
                  const SizedBox(height: 8),
                  Text(
                    'SIGIS',
                    textAlign: TextAlign.center,
                    style: context.textStyles.headlineMedium?.copyWith(
                      color: AppColors.primary,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  Text(
                    'Sistema Integrado de Gestao e Informacao em Saude',
                    textAlign: TextAlign.center,
                    style: context.textStyles.bodySmall,
                  ),
                  const SizedBox(height: 32),
                  TextFormField(
                    controller: _emailController,
                    keyboardType: TextInputType.emailAddress,
                    textInputAction: TextInputAction.next,
                    decoration: const InputDecoration(
                      labelText: 'Matricula ou e-mail',
                      prefixIcon: Icon(Icons.person_outline),
                    ),
                    validator: (value) => (value == null || value.trim().isEmpty)
                        ? 'Informe a matricula ou o e-mail.'
                        : null,
                  ),
                  const SizedBox(height: 16),
                  TextFormField(
                    controller: _passwordController,
                    obscureText: _obscurePassword,
                    textInputAction: TextInputAction.done,
                    onFieldSubmitted: (_) => _submit(),
                    decoration: InputDecoration(
                      labelText: 'Senha',
                      prefixIcon: const Icon(Icons.lock_outline),
                      suffixIcon: IconButton(
                        icon: Icon(
                          _obscurePassword
                              ? Icons.visibility_outlined
                              : Icons.visibility_off_outlined,
                        ),
                        onPressed: () => setState(
                          () => _obscurePassword = !_obscurePassword,
                        ),
                      ),
                    ),
                    validator: (value) => (value == null || value.trim().isEmpty)
                        ? 'Informe a senha.'
                        : null,
                  ),
                  CheckboxListTile(
                    value: _rememberMe,
                    onChanged: (value) =>
                        setState(() => _rememberMe = value ?? false),
                    controlAffinity: ListTileControlAffinity.leading,
                    contentPadding: EdgeInsets.zero,
                    title: const Text('Lembrar-me'),
                  ),
                  const SizedBox(height: 8),
                  ElevatedButton(
                    onPressed: isLoading ? null : _submit,
                    child: isLoading
                        ? const SizedBox(
                            height: 20,
                            width: 20,
                            child: CircularProgressIndicator(
                              strokeWidth: 2,
                              color: Colors.white,
                            ),
                          )
                        : const Text('Entrar'),
                  ),
                ],
              ),
            ),
          ),
        ),
      ),
    );
  }
}

class _LogoPlaceholder extends StatelessWidget {
  const _LogoPlaceholder();

  @override
  Widget build(BuildContext context) {
    return Center(
      child: Container(
        width: 88,
        height: 88,
        decoration: BoxDecoration(
          color: AppColors.primary,
          borderRadius: BorderRadius.circular(24),
        ),
        child: const Icon(Icons.local_hospital, color: Colors.white, size: 44),
      ),
    );
  }
}
