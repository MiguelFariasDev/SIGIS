namespace Sigis.Application.UseCases.Referrals;

/// <summary>Resumo de um encaminhamento entre unidades de serviço, para exibição.</summary>
/// <param name="Id">Identificador do encaminhamento.</param>
/// <param name="PersonId">Identificador da pessoa encaminhada.</param>
/// <param name="OriginUnitId">Identificador da unidade de origem.</param>
/// <param name="OriginUnitName">Nome da unidade de origem.</param>
/// <param name="DestinationUnitId">Identificador da unidade de destino.</param>
/// <param name="DestinationUnitName">Nome da unidade de destino.</param>
/// <param name="Reason">Motivo do encaminhamento (ou da recusa, quando recusado).</param>
/// <param name="Priority">Prioridade do encaminhamento.</param>
/// <param name="ReferralDate">Data e hora (UTC) de criação do encaminhamento.</param>
/// <param name="Status">Situação atual do encaminhamento.</param>
/// <param name="CorrelationId">Identificador de correlação para rastreio.</param>
public sealed record ReferralResponse(
    Guid Id,
    Guid PersonId,
    Guid OriginUnitId,
    string OriginUnitName,
    Guid DestinationUnitId,
    string DestinationUnitName,
    string Reason,
    string Priority,
    DateTime ReferralDate,
    string Status,
    Guid CorrelationId);
