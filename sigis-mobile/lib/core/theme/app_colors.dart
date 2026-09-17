import 'package:flutter/material.dart';

/// Paleta de cores do SIGIS, inspirada na identidade visual do SUS e
/// compartilhada com o frontend web.
abstract final class AppColors {
  /// Azul SUS — cor primaria da marca.
  static const Color primary = Color(0xFF0056A6);

  /// Verde SUS — cor secundaria, usada em confirmacoes e sucesso.
  static const Color secondary = Color(0xFF00A859);

  /// Vermelho SUS — cor de erro e acoes destrutivas (ex.: falta).
  static const Color error = Color(0xFFE30613);

  /// Amarelo SUS — cor de alerta e avisos.
  static const Color warning = Color(0xFFFFCB00);

  /// Cor de superficie clara padrao (branco).
  static const Color surface = Color(0xFFFFFFFF);

  /// Cinza neutro claro, usado em fundos e divisores.
  static const Color neutralLight = Color(0xFFF2F4F7);

  /// Cinza neutro medio, usado em textos secundarios.
  static const Color neutralMedium = Color(0xFF6B7280);

  /// Cinza neutro escuro, usado em textos principais.
  static const Color neutralDark = Color(0xFF1F2937);
}
