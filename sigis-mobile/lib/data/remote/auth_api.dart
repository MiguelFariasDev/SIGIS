import 'package:dio/dio.dart';
import 'package:sigis_mobile/core/config/api_endpoints.dart';

/// Cliente HTTP para o endpoint de autenticacao.
class AuthApi {
  /// Cria o cliente usando o [Dio] configurado pela [ApiClient].
  AuthApi(this._dio);

  final Dio _dio;

  /// Realiza o login com [email] e [password], retornando o corpo bruto
  /// da resposta (`{accessToken, expiresIn, user: {...}}`).
  ///
  /// Lanca [DioException] em caso de falha — a conversao para
  /// [Result]/[AppError] acontece em `AuthRepository`.
  Future<Map<String, dynamic>> login({
    required String email,
    required String password,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.login,
      data: {
        'email': email,
        'password': password,
      },
    );
    return response.data!;
  }
}
