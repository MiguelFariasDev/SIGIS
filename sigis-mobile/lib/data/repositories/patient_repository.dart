import 'package:dio/dio.dart';
import 'package:sigis_mobile/core/errors/app_error.dart';
import 'package:sigis_mobile/core/errors/patient_error.dart';
import 'package:sigis_mobile/core/network/connectivity_service.dart';
import 'package:sigis_mobile/core/result/result.dart';
import 'package:sigis_mobile/data/local/dao/cached_patient_dao.dart';
import 'package:sigis_mobile/data/local/models/cached_patient.dart';
import 'package:sigis_mobile/data/models/patient.dart';
import 'package:sigis_mobile/data/remote/patient_api.dart';

/// Repositorio de pessoas (pacientes).
///
/// Combina busca remota (quando online) com cache local minimizado
/// (regra de LGPD: apenas nome, data de nascimento, CNS e unidade sao
/// mantidos em `cached_patients` — nunca o prontuario completo).
class PatientRepository {
  /// Cria o repositorio com suas dependencias.
  PatientRepository({
    required PatientApi api,
    required CachedPatientDao cachedPatientDao,
    required ConnectivityService connectivity,
  })  : _api = api,
        _cachedPatientDao = cachedPatientDao,
        _connectivity = connectivity;

  final PatientApi _api;
  final CachedPatientDao _cachedPatientDao;
  final ConnectivityService _connectivity;

  /// Busca pessoas pelo [term] (nome, CNS ou CPF).
  ///
  /// Quando online, busca na API e atualiza o cache local. Quando
  /// offline, busca apenas no cache local salvo anteriormente.
  Future<Result<List<Patient>>> search(String term) async {
    if (term.trim().length < 2) {
      return Failure(PatientError.invalidSearchTerm);
    }

    final isOnline = await _connectivity.isOnline();

    if (!isOnline) {
      return _searchLocal(term);
    }

    try {
      final rawResults = await _api.search(term);
      final patients = rawResults
          .map((json) => Patient.fromJson(json as Map<String, dynamic>))
          .toList();

      await _cachedPatientDao.upsertAll(
        patients
            .map(
              (patient) => CachedPatient(
                id: patient.id,
                name: patient.fullName,
                birthDate: patient.birthDate,
                cns: patient.cns,
                unitId: patient.unitId,
                cachedAt: DateTime.now(),
              ),
            )
            .toList(),
      );

      return Success(patients);
    } on DioException catch (exception) {
      return _searchLocal(term, fallbackError: PatientError.fromDio(exception));
    }
  }

  Future<Result<List<Patient>>> _searchLocal(
    String term, {
    AppError? fallbackError,
  }) async {
    final cached = await _cachedPatientDao.search(term);
    if (cached.isEmpty) {
      return Failure(fallbackError ?? PatientError.notFound);
    }
    return Success(
      cached
          .map(
            (c) => Patient(
              id: c.id,
              fullName: c.name,
              birthDate: c.birthDate,
              cns: c.cns,
              unitId: c.unitId,
            ),
          )
          .toList(),
    );
  }

  /// Busca o detalhe completo de uma pessoa pelo [id], incluindo os
  /// atendimentos recentes.
  Future<Result<Patient>> getById(String id) async {
    try {
      final json = await _api.getById(id);
      return Success(Patient.fromJson(json));
    } on DioException catch (exception) {
      return Failure(PatientError.fromDio(exception));
    }
  }
}
