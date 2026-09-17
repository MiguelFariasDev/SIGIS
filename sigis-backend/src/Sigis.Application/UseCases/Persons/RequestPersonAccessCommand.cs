using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Persons;

/// <summary>
/// Solicita acesso completo a um cadastro fora da unidade do profissional
/// (LGPD, art. 11, II) — registra o acesso em auditoria com a justificativa
/// informada e retorna os dados completos da pessoa.
/// </summary>
/// <param name="PersonId">Identificador da pessoa cujos dados serão acessados.</param>
/// <param name="Justificativa">Justificativa obrigatória do acesso.</param>
public sealed record RequestPersonAccessCommand(Guid PersonId, string Justificativa) : IRequest<Result<PersonDetailResponse>>;
