import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:sigis_mobile/core/config/app_config.dart';
import 'package:sigis_mobile/core/network/auth_interceptor.dart';
import 'package:sigis_mobile/core/storage/secure_storage_service.dart';

/// Cria e configura a instancia de [Dio] usada por todos os clientes de
/// API do app.
///
/// Inclui interceptor de autenticacao (Bearer token), log em modo debug
/// e uma tentativa de retry automatico em falhas de rede.
class ApiClient {
  /// Cria o cliente de API, montando o [Dio] com os interceptors
  /// necessarios.
  ApiClient({
    required SecureStorageService secureStorage,
    void Function()? onUnauthorized,
  }) : dio = Dio(
          BaseOptions(
            baseUrl: AppConfig.apiBaseUrl,
            connectTimeout: AppConfig.httpTimeout,
            receiveTimeout: AppConfig.httpTimeout,
          ),
        ) {
    dio.interceptors.add(
      AuthInterceptor(secureStorage, onUnauthorized: onUnauthorized),
    );

    if (kDebugMode) {
      dio.interceptors.add(
        LogInterceptor(requestBody: false, responseBody: false),
      );
    }

    dio.interceptors.add(_RetryOnceInterceptor(dio));
  }

  /// Instancia configurada do Dio, compartilhada por todos os clientes
  /// remotos (`data/remote/*_api.dart`).
  final Dio dio;
}

/// Interceptor que reexecuta a requisicao uma unica vez quando a falha
/// for de conexao (timeout ou rede indisponivel).
class _RetryOnceInterceptor extends Interceptor {
  _RetryOnceInterceptor(this._dio);

  final Dio _dio;

  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    final isConnectionError =
        err.type == DioExceptionType.connectionTimeout ||
            err.type == DioExceptionType.connectionError;
    final alreadyRetried = err.requestOptions.extra['retried'] == true;

    if (isConnectionError && !alreadyRetried) {
      try {
        err.requestOptions.extra['retried'] = true;
        final response = await _dio.fetch(err.requestOptions);
        handler.resolve(response);
        return;
      } on DioException catch (retryError) {
        handler.next(retryError);
        return;
      }
    }

    handler.next(err);
  }
}
