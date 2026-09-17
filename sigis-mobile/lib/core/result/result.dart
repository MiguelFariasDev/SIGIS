import 'package:sigis_mobile/core/errors/app_error.dart';

/// Representa o resultado de uma operacao que pode falhar de forma
/// esperada (sem usar excecoes para controle de fluxo).
///
/// Use [Success] quando a operacao for bem-sucedida e [Failure] quando
/// falhar com um [AppError] conhecido. Excecoes continuam reservadas para
/// bugs de programacao, nunca para fluxos esperados como "paciente nao
/// encontrado" ou "sem conexao".
sealed class Result<T> {
  /// Construtor const da classe base selada.
  const Result();

  /// Retorna `true` quando o resultado e um [Success].
  bool get isSuccess => this is Success<T>;

  /// Retorna `true` quando o resultado e um [Failure].
  bool get isFailure => this is Failure<T>;

  /// Valor de sucesso, ou `null` quando o resultado for [Failure].
  T? get valueOrNull => switch (this) {
        Success<T>(value: final value) => value,
        Failure<T>() => null,
      };

  /// Erro da falha, ou `null` quando o resultado for [Success].
  AppError? get errorOrNull => switch (this) {
        Success<T>() => null,
        Failure<T>(error: final error) => error,
      };

  /// Aplica [onSuccess] ou [onFailure] conforme o tipo do resultado,
  /// retornando o valor produzido pela funcao escolhida.
  R fold<R>(
    R Function(T value) onSuccess,
    R Function(AppError error) onFailure,
  ) =>
      switch (this) {
        Success<T>(value: final value) => onSuccess(value),
        Failure<T>(error: final error) => onFailure(error),
      };
}

/// Resultado de sucesso, carregando o [value] produzido pela operacao.
final class Success<T> extends Result<T> {
  /// Cria um resultado de sucesso com o valor [value].
  const Success(this.value);

  /// Valor produzido pela operacao bem-sucedida.
  final T value;
}

/// Resultado de falha, carregando o [error] que descreve o que houve.
final class Failure<T> extends Result<T> {
  /// Cria um resultado de falha com o erro [error].
  const Failure(this.error);

  /// Erro que descreve a causa da falha.
  final AppError error;
}
