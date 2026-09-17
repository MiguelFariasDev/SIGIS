using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Consents;

/// <summary>Consulta todos os consentimentos (ativos e revogados) de uma pessoa.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
public sealed record GetPersonConsentsQuery(Guid PersonId) : IRequest<Result<IReadOnlyList<PersonConsentResponse>>>;
