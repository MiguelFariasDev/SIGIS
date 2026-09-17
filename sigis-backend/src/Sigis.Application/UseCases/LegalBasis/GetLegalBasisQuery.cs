using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Enums;

namespace Sigis.Application.UseCases.LegalBasis;

/// <summary>
/// Consulta as bases legais (LGPD) que amparam o tratamento de dados por
/// secretaria responsável — conteúdo de referência estático, sem persistência.
/// </summary>
/// <param name="Secretariat">Secretaria a filtrar, opcional — quando omitida, retorna todas.</param>
public sealed record GetLegalBasisQuery(ResponsibleSecretariat? Secretariat)
    : IRequest<Result<IReadOnlyList<LegalBasisResponse>>>;
