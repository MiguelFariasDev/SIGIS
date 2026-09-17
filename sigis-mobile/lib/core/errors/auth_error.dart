import 'package:dio/dio.dart';
import 'package:sigis_mobile/core/errors/app_error.dart';

/// Erros do dominio de autenticacao, agrupados para manter mensagens em
/// portugues e codigos consistentes em toda a aplicacao.
abstract final class AuthError {
  /// Matricula/e-mail ou senha invalidos.
  static const AppError invalidCredentials = AppError(
    code: 'AUTH_001',
    message: 'Matricula ou senha invalidos.',
  );

  /// Campos obrigatorios nao preenchidos no formulario de login.
  static const AppError emptyFields = AppError(
    code: 'AUTH_002',
    message: 'Preencha matricula e senha para continuar.',
  );

  /// Sessao expirada por inatividade ou token invalido/expirado.
  static const AppError sessionExpired = AppError(
    code: 'AUTH_003',
    message: 'Sua sessao expirou. Entre novamente.',
  );

  /// Autenticacao biometrica indisponivel ou nao configurada no
  /// dispositivo.
  static const AppError biometricUnavailable = AppError(
    code: 'AUTH_004',
    message: 'Biometria indisponivel neste dispositivo.',
  );

  /// Falha de comunicacao com o servidor durante a autenticacao.
  static const AppError network = AppError(
    code: 'AUTH_005',
    message: 'Sem conexao com o servidor. Tente novamente.',
  );

  /// Converte uma [DioException] ocorrida durante autenticacao em um
  /// [AppError] apropriado.
  static AppError fromDio(DioException exception) {
    if (exception.type == DioExceptionType.connectionError ||
        exception.type == DioExceptionType.connectionTimeout) {
      return network;
    }
    if (exception.response?.statusCode == 401) {
      return invalidCredentials;
    }
    return AppError.unknown();
  }
}
