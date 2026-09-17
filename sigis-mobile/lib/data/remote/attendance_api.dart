import 'package:dio/dio.dart';
import 'package:sigis_mobile/core/config/api_endpoints.dart';

/// Cliente HTTP para os endpoints de fila e atendimento.
class AttendanceApi {
  /// Cria o cliente usando o [Dio] configurado pela [ApiClient].
  AttendanceApi(this._dio);

  final Dio _dio;

  /// Busca a fila de atendimento de uma unidade.
  Future<List<dynamic>> fetchQueue(String unitId) async {
    final response = await _dio.get<List<dynamic>>(
      ApiEndpoints.filaPorUnidade(unitId),
    );
    return response.data ?? [];
  }

  /// Busca os atendimentos recentes de uma pessoa.
  Future<List<dynamic>> fetchAttendances(String patientId) async {
    final response = await _dio.get<List<dynamic>>(
      ApiEndpoints.atendimentosPorPessoa(patientId),
    );
    return response.data ?? [];
  }

  /// Cria um novo atendimento (usado no registro de sessao rapida).
  Future<Map<String, dynamic>> create(Map<String, dynamic> payload) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.atendimentos,
      data: payload,
    );
    return response.data!;
  }

  /// Registra o comparecimento (ou falta) de um atendimento agendado.
  Future<Map<String, dynamic>> registerComparecimento(
    String id,
    Map<String, dynamic> payload,
  ) async {
    final response = await _dio.patch<Map<String, dynamic>>(
      ApiEndpoints.atendimentoComparecimento(id),
      data: payload,
    );
    return response.data!;
  }
}
