import 'dart:io' show Platform;

/// Configuracoes de ambiente da aplicacao, como a URL base da API.
///
/// Centraliza valores que variam entre plataforma (emulador Android x
/// iOS) e entre ambientes (desenvolvimento x producao), evitando URLs
/// espalhadas pelo codigo.
abstract final class AppConfig {
  /// URL base da API SIGIS.
  ///
  /// Pode ser sobrescrita em tempo de build com
  /// `flutter run --dart-define=API_URL=http://<ip-do-pc>:5000` — necessario
  /// para testar em celular fisico, que nao enxerga nem `localhost` nem
  /// `10.0.2.2` (esses so funcionam no emulador). Sem override: o emulador
  /// Android usa `10.0.2.2` para alcancar o `localhost` da maquina host;
  /// iOS e demais plataformas usam `localhost` diretamente.
  static String get apiBaseUrl {
    const override = String.fromEnvironment('API_URL');
    if (override.isNotEmpty) {
      return override;
    }
    if (Platform.isAndroid) {
      return 'http://10.0.2.2:5000';
    }
    return 'http://localhost:5000';
  }

  /// Tempo limite de conexao e de resposta das requisicoes HTTP.
  static const Duration httpTimeout = Duration(seconds: 30);

  /// Tempo de inatividade apos o qual o usuario e desconectado
  /// automaticamente (regra de LGPD/seguranca).
  static const Duration sessionInactivityTimeout = Duration(minutes: 30);
}
