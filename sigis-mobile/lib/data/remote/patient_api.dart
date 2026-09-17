import 'package:dio/dio.dart';
import 'package:sigis_mobile/core/config/api_endpoints.dart';

/// Cliente HTTP para os endpoints de pessoas (pacientes).
class PatientApi {
  /// Cria o cliente usando o [Dio] configurado pela [ApiClient].
  PatientApi(this._dio);

  final Dio _dio;

  /// Busca pessoas pelo termo informado (nome, CNS ou CPF).
  Future<List<dynamic>> search(String term) async {
    final response = await _dio.get<List<dynamic>>(
      ApiEndpoints.pessoasBuscar,
      queryParameters: {'termo': term},
    );
    return response.data ?? [];
  }

  /// Busca o detalhe completo de uma pessoa pelo [id].
  Future<Map<String, dynamic>> getById(String id) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.pessoaPorId(id),
    );
    return response.data!;
  }
}
