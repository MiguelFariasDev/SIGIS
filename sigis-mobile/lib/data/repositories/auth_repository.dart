import 'dart:convert';

import 'package:dio/dio.dart';
import 'package:sigis_mobile/core/errors/auth_error.dart';
import 'package:sigis_mobile/core/result/result.dart';
import 'package:sigis_mobile/core/storage/preferences_service.dart';
import 'package:sigis_mobile/core/storage/secure_storage_service.dart';
import 'package:sigis_mobile/data/local/database.dart';
import 'package:sigis_mobile/data/models/professional.dart';
import 'package:sigis_mobile/data/remote/auth_api.dart';

/// Repositorio de autenticacao.
///
/// Autentica contra `POST /api/auth/login` no backend real. O backend
/// responde em ingles (`{accessToken, expiresIn, user: {id, name,
/// email, role, unitId, unitAcronym, ...}}`) — a traducao para o modelo
/// interno [Professional] acontece so aqui, no limite da API (mesmo
/// padrao usado no frontend web em `auth/api.ts`).
class AuthRepository {
  /// Cria o repositorio com suas dependencias.
  AuthRepository({
    required AuthApi authApi,
    required SecureStorageService secureStorage,
    required PreferencesService preferences,
    required AppDatabase database,
  })  : _authApi = authApi,
        _secureStorage = secureStorage,
        _preferences = preferences,
        _database = database;

  final AuthApi _authApi;
  final SecureStorageService _secureStorage;
  final PreferencesService _preferences;
  final AppDatabase _database;

  /// Autentica o profissional com [email] e [password].
  ///
  /// Retorna [Success] com o [Professional] autenticado ou [Failure]
  /// com um [AuthError] quando as credenciais forem invalidas, os
  /// campos estiverem vazios ou houver falha de rede.
  Future<Result<Professional>> login({
    required String email,
    required String password,
    bool rememberMe = false,
  }) async {
    if (email.trim().isEmpty || password.trim().isEmpty) {
      return Failure(AuthError.emptyFields);
    }

    try {
      final body = await _authApi.login(email: email, password: password);
      final user = body['user'] as Map<String, dynamic>;
      final professional = Professional(
        id: user['id'] as String,
        name: user['name'] as String,
        email: user['email'] as String,
        unitId: user['unitId'] as String,
        unitAcronym: user['unitAcronym'] as String,
        role: _roleFromWire(user['role'] as String),
      );

      await _secureStorage.saveToken(body['accessToken'] as String);
      await _secureStorage.saveUserId(professional.id);
      await _secureStorage.saveProfessionalJson(jsonEncode({
        'id': professional.id,
        'name': professional.name,
        'email': professional.email,
        'unitId': professional.unitId,
        'unitAcronym': professional.unitAcronym,
        'role': user['role'],
      }));
      await _preferences.setRememberMe(rememberMe);
      await _preferences.setLastLogin(DateTime.now());

      return Success(professional);
    } on DioException catch (exception) {
      return Failure(AuthError.fromDio(exception));
    }
  }

  /// Retorna o profissional atualmente autenticado, reconstituido do
  /// perfil salvo localmente no ultimo login, ou `null` se nao houver
  /// sessao ativa.
  Future<Professional?> currentProfessional() async {
    final token = await _secureStorage.readToken();
    final json = await _secureStorage.readProfessionalJson();
    if (token == null || json == null) {
      return null;
    }

    final user = jsonDecode(json) as Map<String, dynamic>;
    return Professional(
      id: user['id'] as String,
      name: user['name'] as String,
      email: user['email'] as String,
      unitId: user['unitId'] as String,
      unitAcronym: user['unitAcronym'] as String,
      role: _roleFromWire(user['role'] as String),
    );
  }

  /// Encerra a sessao, apagando token, preferencias e todos os dados
  /// locais (regra de LGPD: logout limpa tudo).
  Future<void> logout() async {
    await _secureStorage.clear();
    await _preferences.clear();
    await _database.wipeAllData();
  }

  /// Converte o `role` retornado pelo backend (nome do enum C#, ex.:
  /// `"Coordinator"`) para [ProfessionalRole].
  ProfessionalRole _roleFromWire(String role) => switch (role) {
        'Coordinator' => ProfessionalRole.coordinator,
        'Auditor' => ProfessionalRole.auditor,
        _ => ProfessionalRole.professional,
      };
}
