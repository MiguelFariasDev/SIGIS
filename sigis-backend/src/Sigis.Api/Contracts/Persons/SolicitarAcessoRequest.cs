namespace Sigis.Api.Contracts.Persons;

/// <summary>Requisição de acesso completo a um cadastro fora da unidade do profissional (LGPD, art. 11, II).</summary>
/// <param name="Justificativa">Justificativa obrigatória para o acesso entre unidades/secretarias.</param>
public sealed record SolicitarAcessoRequest(string Justificativa);
