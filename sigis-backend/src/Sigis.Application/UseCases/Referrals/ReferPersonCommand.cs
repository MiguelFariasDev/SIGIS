using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>Encaminha uma pessoa de uma unidade de origem para uma unidade de destino.</summary>
/// <param name="PersonId">Identificador da pessoa encaminhada.</param>
/// <param name="OriginUnitId">Identificador da unidade de origem.</param>
/// <param name="DestinationUnitId">Identificador da unidade de destino.</param>
/// <param name="Reason">Motivo do encaminhamento.</param>
/// <param name="Priority">Prioridade do encaminhamento, como texto (nome do enum <see cref="Sigis.Domain.Enums.QueuePriority"/>).</param>
public sealed record ReferPersonCommand(
    Guid PersonId, Guid OriginUnitId, Guid DestinationUnitId, string? Reason, string Priority)
    : IRequest<Result<ReferralResponse>>;
