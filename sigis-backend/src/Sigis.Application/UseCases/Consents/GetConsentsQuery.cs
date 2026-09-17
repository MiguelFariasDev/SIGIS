using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Enums;

namespace Sigis.Application.UseCases.Consents;

/// <summary>
/// Consulta global de consentimentos LGPD, filtrada opcionalmente por
/// pessoa, tipo, situação de revogação e período — painel DPO/coordenador
/// (T17).
/// </summary>
public sealed record GetConsentsQuery(
    Guid? PersonId, ConsentType? Type, bool? Revoked, DateTime? From, DateTime? To)
    : IRequest<Result<IReadOnlyList<PersonConsentWithPersonResponse>>>;
