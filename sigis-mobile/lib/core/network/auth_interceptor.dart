import 'package:dio/dio.dart';
import 'package:sigis_mobile/core/storage/secure_storage_service.dart';

/// Interceptor Dio que adiciona o token JWT salvo em
/// [SecureStorageService] ao cabecalho `Authorization` de toda
/// requisicao, e limpa a sessao quando o servidor responde 401 (token
/// expirado ou invalido).
class AuthInterceptor extends Interceptor {
  /// Cria o interceptor usando o [SecureStorageService] informado.
  ///
  /// [onUnauthorized], quando informado, e chamado apos limpar a
  /// sessao local em resposta a um 401 — usado para notificar o guard
  /// de rotas (`currentUserProvider`) para redirecionar a `/login`.
  AuthInterceptor(this._secureStorage, {void Function()? onUnauthorized})
      : _onUnauthorized = onUnauthorized;

  final SecureStorageService _secureStorage;
  final void Function()? _onUnauthorized;

  @override
  void onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    final token = await _secureStorage.readToken();
    if (token != null && token.isNotEmpty) {
      options.headers['Authorization'] = 'Bearer $token';
    }
    handler.next(options);
  }

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    if (err.response?.statusCode == 401) {
      await _secureStorage.clear();
      _onUnauthorized?.call();
    }
    handler.next(err);
  }
}
