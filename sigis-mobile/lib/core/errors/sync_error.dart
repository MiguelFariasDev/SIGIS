import 'package:dio/dio.dart';
import 'package:sigis_mobile/core/errors/app_error.dart';

/// Erros do dominio de sincronizacao, agrupados para manter mensagens em
/// portugues e codigos consistentes em toda a aplicacao.
abstract final class SyncError {
  /// Dispositivo sem conexao no momento em que a sincronizacao foi
  /// tentada.
  static const AppError offline = AppError(
    code: 'SYNC_001',
    message: 'Sem conexao. A operacao sera sincronizada automaticamente.',
  );

  /// O servidor rejeitou a operacao por conflito (ex.: falta ja
  /// registrada por outro dispositivo).
  static const AppError conflict = AppError(
    code: 'SYNC_002',
    message: 'Conflito detectado. Revise o registro manualmente.',
  );

  /// Numero maximo de tentativas de sincronizacao atingido.
  static const AppError maxRetriesExceeded = AppError(
    code: 'SYNC_003',
    message: 'Nao foi possivel sincronizar apos varias tentativas.',
  );

  /// Falha de comunicacao com o servidor durante a sincronizacao.
  static const AppError network = AppError(
    code: 'SYNC_004',
    message: 'Falha de conexao durante a sincronizacao.',
  );

  /// Converte uma [DioException] ocorrida durante sincronizacao em um
  /// [AppError] apropriado.
  static AppError fromDio(DioException exception) {
    if (exception.type == DioExceptionType.connectionError ||
        exception.type == DioExceptionType.connectionTimeout) {
      return network;
    }
    if (exception.response?.statusCode == 409) {
      return conflict;
    }
    return AppError.unknown();
  }
}
