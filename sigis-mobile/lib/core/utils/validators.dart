import 'package:cpf_cnpj_validator/cpf_validator.dart';

/// Validacoes de campos de formulario usadas no app.
///
/// Validacao de CPF usa o pacote `cpf_cnpj_validator` — nao
/// reinventamos o algoritmo de digito verificador.
abstract final class Validators {
  /// Retorna `true` quando [value] e um CPF valido (com ou sem
  /// formatacao).
  static bool isValidCpf(String value) => CPFValidator.isValid(value);

  /// Retorna `true` quando [value] e um numero de CNS (Cartao Nacional
  /// de Saude) com 15 digitos numericos.
  ///
  /// O CNS nao possui biblioteca de validacao consolidada no
  /// ecossistema Dart; a checagem aqui e estrutural (quantidade de
  /// digitos), suficiente para o escopo deste app.
  static bool isValidCns(String value) {
    final digits = value.replaceAll(RegExp(r'\D'), '');
    return digits.length == 15;
  }

  /// Retorna `true` quando [value] e um telefone brasileiro valido
  /// (10 ou 11 digitos, com DDD).
  static bool isValidPhone(String value) {
    final digits = value.replaceAll(RegExp(r'\D'), '');
    return digits.length == 10 || digits.length == 11;
  }

  /// Retorna `true` quando [value] nao e vazio apos remover espacos.
  static bool isNotEmpty(String value) => value.trim().isNotEmpty;
}
