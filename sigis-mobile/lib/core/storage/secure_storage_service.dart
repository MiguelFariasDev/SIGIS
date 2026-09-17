import 'package:flutter_secure_storage/flutter_secure_storage.dart';

/// Armazenamento seguro para dados sensiveis, como o token JWT.
///
/// Regra de LGPD: o token de autenticacao NUNCA e salvo em
/// `shared_preferences` — apenas aqui, no armazenamento criptografado do
/// sistema operacional.
class SecureStorageService {
  /// Cria o servico com a instancia de [FlutterSecureStorage] informada
  /// (ou uma padrao, se omitida).
  SecureStorageService({FlutterSecureStorage? storage})
      : _storage = storage ?? const FlutterSecureStorage();

  final FlutterSecureStorage _storage;

  static const String _tokenKey = 'sigis_auth_token';
  static const String _userIdKey = 'sigis_user_id';
  static const String _professionalKey = 'sigis_professional';

  /// Salva o [token] JWT do profissional autenticado.
  Future<void> saveToken(String token) => _storage.write(
        key: _tokenKey,
        value: token,
      );

  /// Le o token JWT salvo, ou `null` se nao houver sessao ativa.
  Future<String?> readToken() => _storage.read(key: _tokenKey);

  /// Salva o identificador do profissional autenticado.
  Future<void> saveUserId(String userId) => _storage.write(
        key: _userIdKey,
        value: userId,
      );

  /// Le o identificador do profissional autenticado, ou `null`.
  Future<String?> readUserId() => _storage.read(key: _userIdKey);

  /// Salva o perfil do profissional autenticado (JSON), para reconstituir
  /// a sessao sem precisar logar novamente a cada abertura do app.
  Future<void> saveProfessionalJson(String json) => _storage.write(
        key: _professionalKey,
        value: json,
      );

  /// Le o perfil do profissional autenticado salvo (JSON), ou `null`.
  Future<String?> readProfessionalJson() =>
      _storage.read(key: _professionalKey);

  /// Remove todos os dados sensiveis armazenados (usado no logout).
  Future<void> clear() => _storage.deleteAll();
}
