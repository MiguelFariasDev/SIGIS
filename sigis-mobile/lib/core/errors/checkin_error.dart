import 'package:dio/dio.dart';
import 'package:sigis_mobile/core/errors/app_error.dart';

/// Erros do dominio de check-in e sessao rapida, agrupados para manter
/// mensagens em portugues e codigos consistentes em toda a aplicacao.
abstract final class CheckInError {
  /// O atendimento informado nao foi encontrado.
  static const AppError attendanceNotFound = AppError(
    code: 'CHECKIN_001',
    message: 'Atendimento nao encontrado.',
  );

  /// A presenca ou falta ja havia sido registrada anteriormente.
  static const AppError alreadyRegistered = AppError(
    code: 'CHECKIN_002',
    message: 'Este atendimento ja possui registro de presenca.',
  );

  /// Campos obrigatorios da sessao rapida nao preenchidos.
  static const AppError missingSessionFields = AppError(
    code: 'CHECKIN_003',
    message: 'Preencha o tipo e o objetivo da sessao.',
  );

  /// Falha de comunicacao com o servidor durante o registro.
  static const AppError network = AppError(
    code: 'CHECKIN_004',
    message: 'Sem conexao. O registro foi salvo e sera sincronizado.',
  );

  /// Converte uma [DioException] ocorrida durante o registro de
  /// check-in ou sessao em um [AppError] apropriado.
  static AppError fromDio(DioException exception) {
    if (exception.type == DioExceptionType.connectionError ||
        exception.type == DioExceptionType.connectionTimeout) {
      return network;
    }
    if (exception.response?.statusCode == 409) {
      return alreadyRegistered;
    }
    if (exception.response?.statusCode == 404) {
      return attendanceNotFound;
    }
    return AppError.unknown();
  }
}
