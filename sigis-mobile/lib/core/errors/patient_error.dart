import 'package:dio/dio.dart';
import 'package:sigis_mobile/core/errors/app_error.dart';

/// Erros do dominio de pacientes (busca e resumo), agrupados para manter
/// mensagens em portugues e codigos consistentes em toda a aplicacao.
abstract final class PatientError {
  /// Nenhum paciente encontrado para o termo buscado.
  static const AppError notFound = AppError(
    code: 'PATIENT_001',
    message: 'Paciente nao encontrado.',
  );

  /// Termo de busca vazio ou invalido.
  static const AppError invalidSearchTerm = AppError(
    code: 'PATIENT_002',
    message: 'Digite ao menos 2 caracteres para buscar.',
  );

  /// Falha de comunicacao com o servidor durante a busca ou consulta.
  static const AppError network = AppError(
    code: 'PATIENT_003',
    message: 'Sem conexao. Mostrando resultados salvos localmente.',
  );

  /// Converte uma [DioException] ocorrida ao consultar pacientes em um
  /// [AppError] apropriado.
  static AppError fromDio(DioException exception) {
    if (exception.type == DioExceptionType.connectionError ||
        exception.type == DioExceptionType.connectionTimeout) {
      return network;
    }
    if (exception.response?.statusCode == 404) {
      return notFound;
    }
    return AppError.unknown();
  }
}
