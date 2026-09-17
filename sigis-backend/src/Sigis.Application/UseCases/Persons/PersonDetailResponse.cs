namespace Sigis.Application.UseCases.Persons;

/// <summary>Dados completos de uma pessoa cadastrada, incluindo responsáveis legais.</summary>
/// <param name="Id">Identificador da pessoa.</param>
/// <param name="Name">Nome completo.</param>
/// <param name="BirthDate">Data de nascimento.</param>
/// <param name="Cns">Número do CNS, quando informado.</param>
/// <param name="Cpf">Número do CPF, quando informado.</param>
/// <param name="MotherName">Nome completo da mãe, quando informado.</param>
/// <param name="Gender">Sexo, quando informado.</param>
/// <param name="RaceColor">Cor/raça, quando informada.</param>
/// <param name="Phone">Telefone de contato, quando informado.</param>
/// <param name="Email">E-mail de contato, quando informado.</param>
/// <param name="Address">Endereço residencial completo, formatado em uma linha, quando informado.</param>
/// <param name="Naturality">Naturalidade, quando informada.</param>
/// <param name="CurrentSchool">Escola atual, quando informada.</param>
/// <param name="Grade">Série escolar, quando informada.</param>
/// <param name="Shift">Turno escolar, quando informado.</param>
/// <param name="ClassGroup">Turma escolar, quando informada.</param>
/// <param name="Zone">Zona de residência, quando informada.</param>
/// <param name="SchoolEnrollment">Matrícula escolar, quando informada.</param>
/// <param name="ReferredBySchool">Se o encaminhamento partiu da escola, quando informado.</param>
/// <param name="DisabilityTypes">Tipos de deficiência, separados por vírgula, quando informados.</param>
/// <param name="NeedsSpecialEducation">Se necessita de AEE, quando informado.</param>
/// <param name="AttendsTutoring">Se frequenta reforço escolar, quando informado.</param>
/// <param name="HasFailedGrade">Se já repetiu alguma série, quando informado.</param>
/// <param name="Guardians">Responsáveis legais vinculados.</param>
/// <param name="CreatedAt">Data e hora (UTC) de criação do cadastro.</param>
/// <param name="UpdatedAt">Data e hora (UTC) da última atualização do cadastro.</param>
public sealed record PersonDetailResponse(
    Guid Id,
    string Name,
    DateOnly BirthDate,
    string? Cns,
    string? Cpf,
    string? MotherName,
    string? Gender,
    string? RaceColor,
    string? Phone,
    string? Email,
    string? Address,
    string? Naturality,
    string? CurrentSchool,
    string? Grade,
    string? Shift,
    string? ClassGroup,
    string? Zone,
    string? SchoolEnrollment,
    bool? ReferredBySchool,
    string? DisabilityTypes,
    bool? NeedsSpecialEducation,
    bool? AttendsTutoring,
    bool? HasFailedGrade,
    IReadOnlyList<GuardianSummary> Guardians,
    DateTime CreatedAt,
    DateTime UpdatedAt);

/// <summary>Resumo de um responsável legal.</summary>
/// <param name="Id">Identificador do responsável.</param>
/// <param name="Name">Nome completo.</param>
/// <param name="Relationship">Grau de parentesco com a pessoa.</param>
/// <param name="Cns">Número do CNS do responsável, quando informado.</param>
/// <param name="BirthDate">Data de nascimento do responsável, quando informada.</param>
public sealed record GuardianSummary(Guid Id, string Name, string Relationship, string? Cns, DateOnly? BirthDate);
