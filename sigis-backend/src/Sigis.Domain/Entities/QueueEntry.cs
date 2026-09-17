using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;

namespace Sigis.Domain.Entities;

/// <summary>
/// Entrada de uma pessoa na fila de atendimento de uma unidade de serviço,
/// para uma especialidade específica. Agregado raiz do módulo de Fila de
/// Atendimento.
/// </summary>
public sealed class QueueEntry : AggregateRoot, IAuditable
{
    /// <summary>Número de faltas consecutivas que aciona o envio automático para busca ativa (RN04).</summary>
    public const int ConsecutiveAbsencesForActiveSearch = 2;

    /// <summary>Comprimento mínimo exigido para a justificativa de reclassificação de prioridade (RN03).</summary>
    public const int MinJustificationLength = 10;

    /// <summary>Identificador da pessoa na fila.</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Identificador da unidade de serviço responsável pela fila.</summary>
    public Guid UnitId { get; private set; }

    /// <summary>Especialidade para a qual a pessoa aguarda atendimento.</summary>
    public string Specialty { get; private set; }

    /// <summary>Prioridade atual da pessoa na fila.</summary>
    public QueuePriority Priority { get; private set; }

    /// <summary>Data e hora (UTC) de entrada na fila.</summary>
    public DateTime EnteredAt { get; private set; }

    /// <summary>Situação atual da entrada na fila.</summary>
    public QueueStatus Status { get; private set; }

    /// <summary>Data e hora (UTC) da última atualização de status.</summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>Número de faltas consecutivas registradas desde o último comparecimento.</summary>
    public int ConsecutiveAbsences { get; private set; }

    private QueueEntry(
        Guid id, Guid personId, Guid unitId, string specialty, QueuePriority priority, DateTime enteredAt)
    {
        Id = id;
        PersonId = personId;
        UnitId = unitId;
        Specialty = specialty;
        Priority = priority;
        EnteredAt = enteredAt;
        UpdatedAt = enteredAt;
        Status = QueueStatus.Waiting;
        ConsecutiveAbsences = 0;
    }

    /// <summary>
    /// Cria uma nova entrada na fila de atendimento, com situação inicial
    /// <see cref="QueueStatus.Waiting"/>.
    /// </summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="unitId">Identificador da unidade de serviço.</param>
    /// <param name="specialty">Especialidade solicitada.</param>
    /// <param name="priority">Prioridade inicial na fila.</param>
    /// <param name="enteredAt">Data e hora (UTC) de entrada na fila.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso, ou de falha com
    /// <see cref="QueueErrors.EspecialidadeObrigatoria"/> quando a
    /// especialidade não for informada.
    /// </returns>
    /// <remarks>
    /// A unicidade de fila ativa por pessoa e unidade (RN01) é garantida pelo
    /// índice único parcial do banco de dados e verificada pela camada de
    /// aplicação antes de chamar este método — não é responsabilidade do
    /// agregado, que não tem acesso a outras entradas da fila. Dentro do
    /// domínio, a regra se expressa por <see cref="IsActive"/>, que a camada
    /// de aplicação usa para decidir se uma nova entrada pode ser criada.
    /// </remarks>
    public static Result<QueueEntry> Create(
        Guid personId, Guid unitId, string? specialty, QueuePriority priority, DateTime enteredAt)
    {
        if (string.IsNullOrWhiteSpace(specialty))
            return Result<QueueEntry>.Failure(QueueErrors.EspecialidadeObrigatoria);

        return Result<QueueEntry>.Success(
            new QueueEntry(Guid.NewGuid(), personId, unitId, specialty.Trim(), priority, enteredAt));
    }

    /// <summary>Indica se a entrada de fila está atualmente ativa (aguardando ou em atendimento).</summary>
    /// <returns>
    /// <see langword="true"/> quando <see cref="Status"/> é
    /// <see cref="QueueStatus.Waiting"/> ou <see cref="QueueStatus.InAttendance"/>.
    /// </returns>
    public bool IsActive() => Status is QueueStatus.Waiting or QueueStatus.InAttendance;

    /// <summary>Tempo decorrido desde a entrada na fila até o instante de referência informado.</summary>
    /// <param name="now">Instante de referência (UTC), injetado para testabilidade.</param>
    /// <returns>Duração desde <see cref="EnteredAt"/> até <paramref name="now"/>.</returns>
    public TimeSpan WaitingTime(DateTime now) => now - EnteredAt;

    /// <summary>
    /// Chama a pessoa para atendimento, transicionando de
    /// <see cref="QueueStatus.Waiting"/> para <see cref="QueueStatus.InAttendance"/>.
    /// </summary>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="QueueErrors.TransicaoInvalida"/> quando a fila não estiver aguardando.
    /// </returns>
    public Result CallForAttendance()
    {
        if (Status != QueueStatus.Waiting)
            return Result.Failure(QueueErrors.TransicaoInvalida);

        Status = QueueStatus.InAttendance;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// Marca a entrada de fila como concluída, encerrando o atendimento e
    /// zerando o contador de faltas consecutivas. Pode ser chamado a partir
    /// de <see cref="QueueStatus.Waiting"/>, <see cref="QueueStatus.InAttendance"/>,
    /// <see cref="QueueStatus.Absent"/> ou <see cref="QueueStatus.ActiveSearch"/> —
    /// uma falta anterior não impede o comparecimento em uma tentativa
    /// posterior (RN04 só afeta a prioridade de busca, não bloqueia o atendimento).
    /// </summary>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="QueueErrors.FilaNaoAtiva"/> quando a fila já estiver concluída.
    /// </returns>
    public Result MarkAsAttended()
    {
        if (Status == QueueStatus.Completed)
            return Result.Failure(QueueErrors.FilaNaoAtiva);

        Status = QueueStatus.Completed;
        ConsecutiveAbsences = 0;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// Registra uma falta ao atendimento agendado. Após
    /// <see cref="ConsecutiveAbsencesForActiveSearch"/> faltas consecutivas, a
    /// entrada é automaticamente enviada para busca ativa (RN04). Pode ser
    /// chamado repetidamente mesmo após uma falta anterior, para permitir a
    /// contagem de faltas consecutivas.
    /// </summary>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="QueueErrors.FilaNaoAtiva"/> quando a fila já estiver concluída.
    /// </returns>
    public Result RegisterAbsence()
    {
        if (Status == QueueStatus.Completed)
            return Result.Failure(QueueErrors.FilaNaoAtiva);

        ConsecutiveAbsences++;
        Status = ConsecutiveAbsences >= ConsecutiveAbsencesForActiveSearch
            ? QueueStatus.ActiveSearch
            : QueueStatus.Absent;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// Envia explicitamente a entrada de fila para busca ativa (ex.:
    /// iniciativa da unidade, independentemente do número de faltas).
    /// </summary>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="QueueErrors.TransicaoInvalida"/> quando a fila já estiver concluída.
    /// </returns>
    public Result SendToActiveSearch()
    {
        if (Status == QueueStatus.Completed)
            return Result.Failure(QueueErrors.TransicaoInvalida);

        Status = QueueStatus.ActiveSearch;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// Reclassifica a prioridade da pessoa na fila, exigindo justificativa
    /// registrada (RN03).
    /// </summary>
    /// <param name="priority">Nova prioridade.</param>
    /// <param name="justification">
    /// Justificativa da reclassificação, com ao menos
    /// <see cref="MinJustificationLength"/> caracteres.
    /// </param>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha quando a justificativa
    /// não for informada ou for muito curta.
    /// </returns>
    public Result ReclassifyPriority(QueuePriority priority, string? justification)
    {
        if (string.IsNullOrWhiteSpace(justification))
            return Result.Failure(QueueErrors.JustificativaObrigatoria);

        if (justification.Trim().Length < MinJustificationLength)
            return Result.Failure(QueueErrors.JustificativaMuitoCurta);

        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }
}
