/// Caminhos dos endpoints da API SIGIS consumidos pelo app mobile.
///
/// Verificados diretamente contra os atributos `[Route]`/`[Http*]` dos
/// controllers reais (`sigis-backend/src/Sigis.Api/Controllers/`) — o
/// backend nao segue um padrao unico de idioma nas rotas (algumas em
/// portugues, outras em ingles), entao cada caminho aqui espelha
/// exatamente o que o controller expoe, nao uma convencao.
abstract final class ApiEndpoints {
  /// Autenticacao do profissional.
  static const String login = '/api/auth/login';

  /// Busca rapida de pessoas por nome, CNS ou CPF (query `termo`).
  static const String pessoasBuscar = '/api/pessoas/busca';

  /// Detalhe de uma pessoa pelo identificador.
  static String pessoaPorId(String id) => '/api/pessoas/$id';

  /// Linha do tempo consolidada de atendimentos de uma pessoa (query
  /// opcionais `nivel`/`justificativa`) — nao consumida pelo mobile hoje.
  static String pessoaTimeline(String id) => '/api/pessoas/$id/linha-do-tempo';

  /// Fila de atendimento de uma unidade (fila do dia).
  static String filaPorUnidade(String unidadeId) =>
      '/api/unidades/$unidadeId/fila';

  /// Registra o comparecimento (ou falta) de um atendimento.
  static String atendimentoComparecimento(String id) =>
      '/api/atendimentos/$id/comparecimento';

  /// Atendimentos recentes de uma pessoa.
  static String atendimentosPorPessoa(String pessoaId) =>
      '/api/atendimentos/pessoa/$pessoaId';

  /// Criacao de um novo atendimento.
  static const String atendimentos = '/api/atendimentos';
}
