import 'package:flutter/material.dart';

/// Atalhos convenientes sobre [BuildContext], usados nas telas do app.
extension ContextExtensions on BuildContext {
  /// Atalho para o tema atual.
  ThemeData get theme => Theme.of(this);

  /// Atalho para o esquema de cores atual.
  ColorScheme get colors => Theme.of(this).colorScheme;

  /// Atalho para os estilos de texto atuais.
  TextTheme get textStyles => Theme.of(this).textTheme;

  /// Exibe uma snackbar simples com [message].
  void showSnackBar(String message, {Color? backgroundColor}) {
    ScaffoldMessenger.of(this).showSnackBar(
      SnackBar(
        content: Text(message),
        backgroundColor: backgroundColor,
      ),
    );
  }
}
