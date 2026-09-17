using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Enums;
using Sigis.Domain.Events;

namespace Sigis.Domain.Entities;

/// <summary>
/// Encaminhamento de uma pessoa entre unidades de serviço da rede, com
/// motivo e prioridade. Agregado raiz do módulo de Atendimentos e Histórico.
/// </summary>
public sealed class Referral : AggregateRoot
{
    /// <summary>Comprimento mínimo exigido para o motivo do encaminhamento (RN — análoga à RN03 da fila).</summary>
    public const int MinReasonLength = 10;

    /// <summary>Identificador da pessoa encaminhada.</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Identificador da unidade de origem do encaminhamento.</summary>
    public Guid OriginUnitId { get; private set; }

    /// <summary>Identificador da unidade de destino do encaminhamento.</summary>
    public Guid DestinationUnitId { get; private set; }

    /// <summary>Motivo do encaminhamento.</summary>
    public string Reason { get; private set; }

    /// <summary>Prioridade do encaminhamento.</summary>
    public QueuePriority Priority { get; private set; }

    /// <summary>Data e hora (UTC) em que o encaminhamento foi criado.</summary>
    public DateTime ReferralDate { get; private set; }

    /// <summary>Situação atual do encaminhamento.</summary>
    public ReferralStatus Status { get; private set; }

    /// <summary>
    /// Identificador de correlação, usado para rastrear este encaminhamento
    /// através de eventos de domínio e integrações externas.
    /// </summary>
    public Guid CorrelationId { get; private set; }

    private Referral(
        Guid id, Guid personId, Guid originUnitId, Guid destinationUnitId, string reason, QueuePriority priority,
        DateTime referralDate, Guid correlationId)
    {
        Id = id;
        PersonId = personId;
        OriginUnitId = originUnitId;
        DestinationUnitId = destinationUnitId;
        Reason = reason;
        Priority = priority;
        ReferralDate = referralDate;
        Status = ReferralStatus.Pending;
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Cria um novo encaminhamento, com situação inicial
    /// <see cref="ReferralStatus.Pending"/>, emitindo
    /// <see cref="ReferralCreatedEvent"/>.
    /// </summary>
    /// <param name="personId">Identificador da pessoa encaminhada.</param>
    /// <param name="originUnitId">Identificador da unidade de origem.</param>
    /// <param name="destinationUnitId">Identificador da unidade de destino.</param>
    /// <param name="reason">Motivo do encaminhamento, com ao menos <see cref="MinReasonLength"/> caracteres.</param>
    /// <param name="priority">Prioridade do encaminhamento.</param>
    /// <param name="referralDate">Data e hora (UTC) do encaminhamento.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso, ou de falha quando origem e
    /// destino forem iguais, ou o motivo não for informado ou for muito curto.
    /// </returns>
    public static Result<Referral> Create(
        Guid personId,
        Guid originUnitId,
        Guid destinationUnitId,
        string? reason,
        QueuePriority priority,
        DateTime referralDate)
    {
        if (originUnitId == destinationUnitId)
            return Result<Referral>.Failure(ReferralErrors.OrigemIgualDestino);

        if (string.IsNullOrWhiteSpace(reason))
            return Result<Referral>.Failure(ReferralErrors.MotivoObrigatorio);

        if (reason.Trim().Length < MinReasonLength)
            return Result<Referral>.Failure(ReferralErrors.MotivoMuitoCurto);

        var referral = new Referral(
            Guid.NewGuid(), personId, originUnitId, destinationUnitId, reason.Trim(), priority, referralDate,
            Guid.NewGuid());

        referral.Raise(new ReferralCreatedEvent(referral.Id, personId, originUnitId, destinationUnitId));

        return Result<Referral>.Success(referral);
    }

    /// <summary>
    /// Aceita o encaminhamento, transicionando de
    /// <see cref="ReferralStatus.Pending"/> para <see cref="ReferralStatus.Accepted"/>.
    /// </summary>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="ReferralErrors.TransicaoInvalida"/> quando o encaminhamento
    /// não estiver pendente.
    /// </returns>
    public Result Accept()
    {
        if (Status != ReferralStatus.Pending)
            return Result.Failure(ReferralErrors.TransicaoInvalida);

        Status = ReferralStatus.Accepted;
        return Result.Success();
    }

    /// <summary>
    /// Registra o primeiro atendimento na unidade de destino, concluindo o
    /// encaminhamento (RN05) e emitindo <see cref="ReferralCompletedEvent"/>.
    /// </summary>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="ReferralErrors.NaoAceito"/> quando o encaminhamento ainda
    /// não tiver sido aceito.
    /// </returns>
    public Result RegisterFirstAttendance()
    {
        if (Status != ReferralStatus.Accepted)
            return Result.Failure(ReferralErrors.NaoAceito);

        Status = ReferralStatus.Completed;
        Raise(new ReferralCompletedEvent(Id, PersonId));
        return Result.Success();
    }

    /// <summary>
    /// Recusa o encaminhamento, transicionando de
    /// <see cref="ReferralStatus.Pending"/> para <see cref="ReferralStatus.Refused"/>.
    /// </summary>
    /// <param name="reason">Motivo da recusa.</param>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="ReferralErrors.TransicaoInvalida"/> quando o encaminhamento
    /// não estiver pendente, ou com <see cref="ReferralErrors.MotivoObrigatorio"/>
    /// quando o motivo da recusa não for informado.
    /// </returns>
    public Result Refuse(string? reason)
    {
        if (Status != ReferralStatus.Pending)
            return Result.Failure(ReferralErrors.TransicaoInvalida);

        if (string.IsNullOrWhiteSpace(reason))
            return Result.Failure(ReferralErrors.MotivoObrigatorio);

        Status = ReferralStatus.Refused;
        Reason = reason.Trim();
        return Result.Success();
    }
}
