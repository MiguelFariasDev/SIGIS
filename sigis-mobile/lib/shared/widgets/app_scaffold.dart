import 'package:flutter/material.dart';
import 'package:sigis_mobile/shared/widgets/sync_indicator.dart';

/// Scaffold padrao das telas autenticadas do SIGIS, com AppBar
/// consistente (titulo, logo, indicador de sincronizacao) e corpo
/// flexivel.
class AppScaffold extends StatelessWidget {
  /// Cria o scaffold com [title] e [body].
  const AppScaffold({
    super.key,
    required this.title,
    required this.body,
    this.actions,
    this.floatingActionButton,
    this.showSyncIndicator = true,
    this.automaticallyImplyLeading = true,
  });

  /// Titulo exibido na AppBar.
  final String title;

  /// Conteudo principal da tela.
  final Widget body;

  /// Acoes adicionais na AppBar, exibidas antes do [SyncIndicator].
  final List<Widget>? actions;

  /// Botao flutuante opcional da tela.
  final Widget? floatingActionButton;

  /// Quando `true`, exibe o [SyncIndicator] na AppBar.
  final bool showSyncIndicator;

  /// Quando `true`, exibe o botao de voltar automatico da AppBar.
  final bool automaticallyImplyLeading;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        automaticallyImplyLeading: automaticallyImplyLeading,
        title: Text(title),
        actions: [
          ...?actions,
          if (showSyncIndicator) const SyncIndicator(),
        ],
      ),
      body: SafeArea(child: body),
      floatingActionButton: floatingActionButton,
    );
  }
}
