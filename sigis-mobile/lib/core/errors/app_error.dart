/// Erro de aplicacao generico, com um [code] estavel para diagnostico e
/// uma [message] em portugues pronta para exibicao ao usuario.
///
/// Erros de dominio especificos (ex.: [AuthError], [SyncError]) expoem
/// construtores nomeados que produzem instancias de [AppError] com
/// [code] e [message] ja preenchidos, mantendo uma unica forma de
/// carregar erro por toda a aplicacao.
class AppError {
  /// Cria um erro de aplicacao com [code] tecnico e [message] em pt-BR.
  const AppError({required this.code, required this.message});

  /// Codigo estavel do erro, usado para logs e diagnostico (ex.:
  /// "AUTH_001").
  final String code;

  /// Mensagem amigavel em portugues, pronta para exibir ao usuario.
  final String message;

  /// Erro generico e inesperado, usado como ultimo recurso quando a
  /// causa nao se encaixa em nenhuma categoria conhecida.
  factory AppError.unknown() => const AppError(
        code: 'APP_000',
        message: 'Ocorreu um erro inesperado. Tente novamente.',
      );

  @override
  String toString() => '[$code] $message';

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is AppError && other.code == code && other.message == message;

  @override
  int get hashCode => Object.hash(code, message);
}
