import 'package:shared_preferences/shared_preferences.dart';

/// Configuracoes locais nao sensiveis, persistidas com
/// `shared_preferences`.
///
/// Nunca armazenar aqui tokens, senhas ou dados de pacientes — apenas
/// preferencias de interface, como "lembrar-me" e a ultima unidade
/// selecionada.
class PreferencesService {
  /// Cria o servico de preferencias.
  const PreferencesService();

  static const String _rememberMeKey = 'sigis_remember_me';
  static const String _lastLoginKey = 'sigis_last_login';

  /// Salva a preferencia "lembrar-me" do login.
  Future<void> setRememberMe(bool value) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setBool(_rememberMeKey, value);
  }

  /// Le a preferencia "lembrar-me" do login (padrao `false`).
  Future<bool> getRememberMe() async {
    final prefs = await SharedPreferences.getInstance();
    return prefs.getBool(_rememberMeKey) ?? false;
  }

  /// Salva o instante do ultimo login bem-sucedido.
  Future<void> setLastLogin(DateTime dateTime) async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.setString(_lastLoginKey, dateTime.toIso8601String());
  }

  /// Remove todas as preferencias locais (usado no logout).
  Future<void> clear() async {
    final prefs = await SharedPreferences.getInstance();
    await prefs.remove(_rememberMeKey);
    await prefs.remove(_lastLoginKey);
  }
}
