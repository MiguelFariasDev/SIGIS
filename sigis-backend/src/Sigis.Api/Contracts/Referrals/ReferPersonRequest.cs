namespace Sigis.Api.Contracts.Referrals;

/// <summary>Requisição de encaminhamento de uma pessoa entre unidades de serviço.</summary>
/// <param name="PersonId">Identificador da pessoa a encaminhar.</param>
/// <param name="OriginUnitId">Identificador da unidade de origem.</param>
/// <param name="DestinationUnitId">Identificador da unidade de destino.</param>
/// <param name="Reason">Motivo do encaminhamento.</param>
/// <param name="Priority">Prioridade do encaminhamento (nome do enum <see cref="Sigis.Domain.Enums.QueuePriority"/>).</param>
public sealed record ReferPersonRequest(
    Guid PersonId, Guid OriginUnitId, Guid DestinationUnitId, string? Reason, string Priority);
