import 'package:connectivity_plus/connectivity_plus.dart';

/// Servico de deteccao de conectividade de rede.
///
/// Usado pelo [SyncService] para decidir entre tentar sincronizar
/// imediatamente ou aguardar a conexao voltar.
class ConnectivityService {
  /// Cria o servico com a instancia de [Connectivity] informada (ou uma
  /// padrao, se omitida).
  ConnectivityService({Connectivity? connectivity})
      : _connectivity = connectivity ?? Connectivity();

  final Connectivity _connectivity;

  /// Stream que emite `true` quando o dispositivo esta online e `false`
  /// quando esta offline.
  Stream<bool> get onStatusChange => _connectivity.onConnectivityChanged
      .map((results) => _hasConnection(results));

  /// Verifica de forma pontual se o dispositivo esta online.
  Future<bool> isOnline() async {
    final results = await _connectivity.checkConnectivity();
    return _hasConnection(results);
  }

  bool _hasConnection(List<ConnectivityResult> results) =>
      results.any((result) => result != ConnectivityResult.none);
}
