using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Enums;
using Sigis.Domain.Events;

namespace Sigis.Domain.Entities;

/// <summary>
/// Registro de uma sessão de atendimento de uma pessoa em uma unidade de
/// serviço, com dados específicos do tipo de sessão armazenados como JSON
/// (ver seção "Por que dados_formulario é JSONB" do modelo de dados).
/// Agregado raiz do módulo de Atendimentos e Histórico.
/// </summary>
public sealed class Attendance : AggregateRoot
{
    /// <summary>Identificador da pessoa atendida.</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Identificador da unidade de serviço onde ocorre o atendimento.</summary>
    public Guid UnitId { get; private set; }

    /// <summary>Identificador do profissional responsável pelo atendimento.</summary>
    public Guid ProfessionalId { get; private set; }

    /// <summary>Data e hora (UTC) do atendimento.</summary>
    public DateTime DateTime { get; private set; }

    /// <summary>Tipo de sessão/anamnese registrada, específico do serviço.</summary>
    public SessionType SessionType { get; private set; }

    /// <summary>Situação de comparecimento do atendimento.</summary>
    public AttendanceStatus Status { get; private set; }

    /// <summary>Dados do formulário específico do tipo de sessão, em formato JSON.</summary>
    public string? FormData { get; private set; }

    /// <summary>Número sequencial da sessão para a mesma pessoa e tipo de sessão.</summary>
    public int SessionNumber { get; private set; }

    /// <summary>Identificador do profissional que realizou a triagem, quando informado (NAPE A.2/A.3).</summary>
    public Guid? TriagedByProfessionalId { get; private set; }

    /// <summary>Queixa principal relatada na triagem, quando informada (NAPE A.2/A.3).</summary>
    public string? MainComplaint { get; private set; }

    /// <summary>Data e hora (UTC) de criação do registro.</summary>
    public DateTime CreatedAt { get; private set; }

    private Attendance(
        Guid id, Guid personId, Guid unitId, Guid professionalId, DateTime dateTime,
        SessionType sessionType, int sessionNumber, string? formData, DateTime createdAt,
        Guid? triagedByProfessionalId, string? mainComplaint)
    {
        Id = id;
        PersonId = personId;
        UnitId = unitId;
        ProfessionalId = professionalId;
        DateTime = dateTime;
        SessionType = sessionType;
        Status = AttendanceStatus.Scheduled;
        FormData = formData;
        SessionNumber = sessionNumber;
        CreatedAt = createdAt;
        TriagedByProfessionalId = triagedByProfessionalId;
        MainComplaint = mainComplaint;
    }

    /// <summary>
    /// Cria um novo registro de atendimento, com situação inicial
    /// <see cref="AttendanceStatus.Scheduled"/>.
    /// </summary>
    /// <param name="personId">Identificador da pessoa atendida.</param>
    /// <param name="unitId">Identificador da unidade de serviço.</param>
    /// <param name="professionalId">Identificador do profissional responsável.</param>
    /// <param name="dateTime">Data e hora (UTC) do atendimento.</param>
    /// <param name="sessionType">Tipo de sessão/anamnese.</param>
    /// <param name="sessionNumber">Número sequencial da sessão.</param>
    /// <param name="createdAt">Data e hora (UTC) de criação do registro.</param>
    /// <param name="formData">Dados do formulário específico, em JSON, opcional na criação.</param>
    /// <param name="triagedByProfessionalId">Identificador do profissional que triou, opcional.</param>
    /// <param name="mainComplaint">Queixa principal relatada na triagem, opcional.</param>
    /// <returns>Um <see cref="Result{T}"/> sempre bem-sucedido com o atendimento agendado.</returns>
    public static Result<Attendance> Create(
        Guid personId,
        Guid unitId,
        Guid professionalId,
        DateTime dateTime,
        SessionType sessionType,
        int sessionNumber,
        DateTime createdAt,
        string? formData = null,
        Guid? triagedByProfessionalId = null,
        string? mainComplaint = null)
    {
        var attendance = new Attendance(
            Guid.NewGuid(), personId, unitId, professionalId, dateTime, sessionType, sessionNumber, formData,
            createdAt, triagedByProfessionalId, mainComplaint?.Trim());

        return Result<Attendance>.Success(attendance);
    }

    /// <summary>
    /// Registra os dados de triagem do atendimento (quem triou e a queixa
    /// principal) — NAPE A.2/A.3.
    /// </summary>
    /// <param name="triagedByProfessionalId">Identificador do profissional que realizou a triagem.</param>
    /// <param name="mainComplaint">Queixa principal relatada.</param>
    /// <returns>Um <see cref="Result"/> sempre bem-sucedido.</returns>
    public Result RegisterTriage(Guid triagedByProfessionalId, string? mainComplaint)
    {
        TriagedByProfessionalId = triagedByProfessionalId;
        MainComplaint = mainComplaint?.Trim();
        return Result.Success();
    }

    /// <summary>
    /// Registra o comparecimento da pessoa ao atendimento, emitindo
    /// <see cref="AttendanceRegisteredEvent"/>.
    /// </summary>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="AttendanceErrors.ComparecimentoJaRegistrado"/> quando o
    /// comparecimento (ou a falta) já tiver sido registrado.
    /// </returns>
    public Result MarkAsAttended()
    {
        if (Status != AttendanceStatus.Scheduled)
            return Result.Failure(AttendanceErrors.ComparecimentoJaRegistrado);

        Status = AttendanceStatus.Attended;
        Raise(new AttendanceRegisteredEvent(Id, PersonId, UnitId));
        return Result.Success();
    }

    /// <summary>
    /// Registra a falta da pessoa ao atendimento, emitindo
    /// <see cref="AbsenceRegisteredEvent"/>.
    /// </summary>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="AttendanceErrors.ComparecimentoJaRegistrado"/> quando o
    /// comparecimento (ou a falta) já tiver sido registrado.
    /// </returns>
    public Result MarkAsAbsent()
    {
        if (Status != AttendanceStatus.Scheduled)
            return Result.Failure(AttendanceErrors.ComparecimentoJaRegistrado);

        Status = AttendanceStatus.Absent;
        Raise(new AbsenceRegisteredEvent(Id, PersonId, UnitId));
        return Result.Success();
    }

    /// <summary>
    /// Atualiza os dados do formulário específico do tipo de sessão, apenas
    /// enquanto o atendimento ainda estiver agendado.
    /// </summary>
    /// <param name="formDataJson">Novo conteúdo do formulário, em JSON.</param>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="AttendanceErrors.NaoPodeAlterarAposComparecimento"/> quando o
    /// comparecimento já tiver sido registrado, ou com
    /// <see cref="AttendanceErrors.DadosFormularioInvalidos"/> quando o JSON
    /// informado for vazio.
    /// </returns>
    public Result UpdateFormData(string? formDataJson)
    {
        if (Status != AttendanceStatus.Scheduled)
            return Result.Failure(AttendanceErrors.NaoPodeAlterarAposComparecimento);

        if (string.IsNullOrWhiteSpace(formDataJson))
            return Result.Failure(AttendanceErrors.DadosFormularioInvalidos);

        FormData = formDataJson;
        return Result.Success();
    }
}
