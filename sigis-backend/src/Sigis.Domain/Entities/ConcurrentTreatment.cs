using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using DayOfWeek = Sigis.Domain.Enums.DayOfWeek;

namespace Sigis.Domain.Entities;

/// <summary>
/// Atendimento concomitante que uma pessoa recebe em outro serviço/
/// profissional, fora da rede de origem do registro (NAPE, ficha A.4 —
/// tabela "Atendimentos concomitantes"). Relação 1:N com <see cref="Person"/>.
/// </summary>
public sealed class ConcurrentTreatment : Entity
{
    /// <summary>Identificador da pessoa.</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Especialidade do profissional (ex.: "Terapeuta Ocupacional", "Psicólogo").</summary>
    public string Specialty { get; private set; }

    /// <summary>Local do atendimento (ex.: "Casa Mais Azul", "NAPE").</summary>
    public string Location { get; private set; }

    /// <summary>Nome do profissional responsável.</summary>
    public string ProfessionalName { get; private set; }

    /// <summary>Dia da semana em que o atendimento ocorre.</summary>
    public DayOfWeek DayOfWeek { get; private set; }

    /// <summary>Horário de início.</summary>
    public TimeOnly StartTime { get; private set; }

    /// <summary>Horário de término.</summary>
    public TimeOnly EndTime { get; private set; }

    /// <summary>Observações, quando informadas.</summary>
    public string? Notes { get; private set; }

    /// <summary>Data e hora (UTC) de criação do registro.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Data e hora (UTC) da última atualização do registro.</summary>
    public DateTime UpdatedAt { get; private set; }

    private ConcurrentTreatment(
        Guid id, Guid personId, string specialty, string location, string professionalName,
        DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, string? notes, DateTime createdAt)
    {
        Id = id;
        PersonId = personId;
        Specialty = specialty;
        Location = location;
        ProfessionalName = professionalName;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        Notes = notes;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Registra um atendimento concomitante para a pessoa.
    /// </summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="specialty">Especialidade do profissional (RN-CT03).</param>
    /// <param name="location">Local do atendimento.</param>
    /// <param name="professionalName">Nome do profissional responsável.</param>
    /// <param name="dayOfWeek">Dia da semana do atendimento.</param>
    /// <param name="startTime">Horário de início (RN-CT01: anterior ao fim).</param>
    /// <param name="endTime">Horário de término.</param>
    /// <param name="createdAt">Data e hora (UTC) de criação.</param>
    /// <param name="notes">Observações, opcionais.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso, ou de falha quando a
    /// especialidade/local não forem informados ou o horário de início não
    /// for anterior ao de término.
    /// </returns>
    /// <remarks>
    /// RN-CT02 (sem sobreposição de horário no mesmo dia) depende de
    /// consultar os demais tratamentos já cadastrados para a pessoa e é
    /// responsabilidade da camada de aplicação/repositório antes de chamar
    /// este método.
    /// </remarks>
    public static Result<ConcurrentTreatment> Create(
        Guid personId,
        string? specialty,
        string? location,
        string? professionalName,
        DayOfWeek dayOfWeek,
        TimeOnly startTime,
        TimeOnly endTime,
        DateTime createdAt,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(specialty))
            return Result<ConcurrentTreatment>.Failure(ConcurrentTreatmentErrors.EspecialidadeObrigatoria);

        if (string.IsNullOrWhiteSpace(location))
            return Result<ConcurrentTreatment>.Failure(ConcurrentTreatmentErrors.LocalObrigatorio);

        if (startTime >= endTime)
            return Result<ConcurrentTreatment>.Failure(ConcurrentTreatmentErrors.HorarioInicioAposFim);

        return Result<ConcurrentTreatment>.Success(new ConcurrentTreatment(
            Guid.NewGuid(), personId, specialty.Trim(), location.Trim(), professionalName?.Trim() ?? string.Empty,
            dayOfWeek, startTime, endTime, notes?.Trim(), createdAt));
    }

    /// <summary>
    /// Indica se este atendimento se sobrepõe, no mesmo dia da semana, ao
    /// intervalo informado — usado pela camada de aplicação para checar RN-CT02
    /// contra os demais tratamentos já cadastrados.
    /// </summary>
    /// <param name="dayOfWeek">Dia da semana a comparar.</param>
    /// <param name="startTime">Início do intervalo a comparar.</param>
    /// <param name="endTime">Fim do intervalo a comparar.</param>
    /// <returns><see langword="true"/> quando os intervalos se sobrepõem no mesmo dia.</returns>
    public bool OverlapsWith(DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime)
        => DayOfWeek == dayOfWeek && StartTime < endTime && startTime < EndTime;

    /// <summary>Atualiza os dados do atendimento concomitante.</summary>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha quando a
    /// especialidade/local não forem informados ou o horário de início não
    /// for anterior ao de término.
    /// </returns>
    public Result UpdateData(
        string? specialty, string? location, string? professionalName,
        DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, string? notes)
    {
        if (string.IsNullOrWhiteSpace(specialty))
            return Result.Failure(ConcurrentTreatmentErrors.EspecialidadeObrigatoria);

        if (string.IsNullOrWhiteSpace(location))
            return Result.Failure(ConcurrentTreatmentErrors.LocalObrigatorio);

        if (startTime >= endTime)
            return Result.Failure(ConcurrentTreatmentErrors.HorarioInicioAposFim);

        Specialty = specialty.Trim();
        Location = location.Trim();
        ProfessionalName = professionalName?.Trim() ?? string.Empty;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        Notes = notes?.Trim();
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}
