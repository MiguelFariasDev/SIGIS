using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Events;
using Sigis.Domain.Interfaces;
using Sigis.Domain.ValueObjects;

namespace Sigis.Domain.Entities;

/// <summary>
/// Núcleo de identidade canônica de uma pessoa com TEA (ou em investigação)
/// atendida pela rede municipal. É o agregado raiz do módulo de Cadastro
/// Único — compartilhado por todos os serviços da rede (NASF, NAPE, CREAES,
/// Casa Mais Azul, CRASF), em contraste com os registros de atendimento
/// específicos de cada serviço (ver <see cref="Attendance"/>).
/// </summary>
public sealed class Person : AggregateRoot, IAuditable
{
    private readonly List<Guardian> _guardians = [];

    /// <summary>Nome completo da pessoa.</summary>
    public PersonName Name { get; private set; }

    /// <summary>Data de nascimento da pessoa.</summary>
    public DateOnly BirthDate { get; private set; }

    /// <summary>Número do Cartão Nacional de Saúde, quando informado.</summary>
    public Cns? Cns { get; private set; }

    /// <summary>Número do CPF, quando informado.</summary>
    public Cpf? Cpf { get; private set; }

    /// <summary>Nome completo da mãe, quando informado.</summary>
    public string? MotherName { get; private set; }

    /// <summary>Sexo da pessoa, quando informado.</summary>
    public string? Gender { get; private set; }

    /// <summary>Cor/raça da pessoa, quando informada.</summary>
    public string? RaceColor { get; private set; }

    /// <summary>Telefone de contato, quando informado.</summary>
    public PhoneNumber? Phone { get; private set; }

    /// <summary>Endereço de e-mail de contato, quando informado.</summary>
    public EmailAddress? Email { get; private set; }

    /// <summary>Endereço residencial, quando informado.</summary>
    public Address? Address { get; private set; }

    /// <summary>Responsáveis legais vinculados à pessoa.</summary>
    public IReadOnlyCollection<Guardian> Guardians => _guardians.AsReadOnly();

    /// <summary>Naturalidade (cidade/estado de nascimento), quando informada (NAPE A.2/A.3).</summary>
    public string? Naturality { get; private set; }

    /// <summary>Escola atual, quando informada (NAPE A.2/A.3/A.4).</summary>
    public string? CurrentSchool { get; private set; }

    /// <summary>Série escolar atual, quando informada (NAPE A.2/A.3/A.4).</summary>
    public string? Grade { get; private set; }

    /// <summary>Turno escolar, quando informado (NAPE A.2/A.3/A.4).</summary>
    public string? Shift { get; private set; }

    /// <summary>Turma escolar, quando informada (NAPE A.2/A.3).</summary>
    public string? ClassGroup { get; private set; }

    /// <summary>Zona de residência ("Urbana"/"Rural"), quando informada (NAPE A.2/A.3).</summary>
    public string? Zone { get; private set; }

    /// <summary>Número de matrícula escolar, quando informado.</summary>
    public string? SchoolEnrollment { get; private set; }

    /// <summary>Indica se o encaminhamento partiu da escola, quando informado (NAPE A.2).</summary>
    public bool? ReferredBySchool { get; private set; }

    /// <summary>
    /// Tipos de deficiência associados à pessoa, separados por vírgula
    /// (ex.: "TEA, TDAH") — NAPE A.4. Decisão de hackathon: armazenar como
    /// CSV em vez de uma tabela N:N, dado o volume baixo de combinações e o
    /// prazo do projeto (RN32); evolução futura natural seria uma tabela de
    /// associação <c>person_disability</c>.
    /// </summary>
    public string? DisabilityTypes { get; private set; }

    /// <summary>Indica se a pessoa necessita de Atendimento Educacional Especializado (AEE), quando informado.</summary>
    public bool? NeedsSpecialEducation { get; private set; }

    /// <summary>Indica se a pessoa frequenta reforço escolar, quando informado.</summary>
    public bool? AttendsTutoring { get; private set; }

    /// <summary>Indica se a pessoa já repetiu alguma série, quando informado (NAPE A.3).</summary>
    public bool? HasFailedGrade { get; private set; }

    /// <summary>Data e hora (UTC) de criação do cadastro.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Data e hora (UTC) da última atualização do cadastro.</summary>
    public DateTime UpdatedAt { get; private set; }

    private Person(
        Guid id,
        PersonName name,
        DateOnly birthDate,
        Cns? cns,
        Cpf? cpf,
        string? motherName,
        string? gender,
        string? raceColor,
        PhoneNumber? phone,
        EmailAddress? email,
        Address? address,
        DateTime createdAt)
    {
        Id = id;
        Name = name;
        BirthDate = birthDate;
        Cns = cns;
        Cpf = cpf;
        MotherName = motherName;
        Gender = gender;
        RaceColor = raceColor;
        Phone = phone;
        Email = email;
        Address = address;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Construtor privado sem parâmetros exigido pelo Entity Framework Core
    /// para materializar a entidade a partir do banco: <see cref="Address"/>
    /// é uma owned entity (navegação), e navegações não podem ser vinculadas
    /// via injeção de construtor — apenas propriedades escalares (incluindo
    /// as convertidas por <c>ValueConverter</c>, como <see cref="Name"/>).
    /// Os demais valores são preenchidos pelo próprio EF Core logo em
    /// seguida, via reflexão sobre os setters privados.
    /// </summary>
#pragma warning disable CS8618
    private Person()
    {
    }
#pragma warning restore CS8618

    /// <summary>
    /// Cria uma nova <see cref="Person"/>, validando a data de nascimento e
    /// emitindo <see cref="PersonRegisteredEvent"/>.
    /// </summary>
    /// <param name="name">Nome completo, já validado como <see cref="PersonName"/>.</param>
    /// <param name="birthDate">Data de nascimento.</param>
    /// <param name="today">
    /// Data de referência (local do fuso do usuário ou UTC, a critério da
    /// camada de aplicação) usada para validar a data de nascimento —
    /// recebida como parâmetro em vez de lida internamente, para manter o
    /// domínio testável e livre de dependência do relógio do sistema.
    /// </param>
    /// <param name="cns">Número do CNS, opcional.</param>
    /// <param name="cpf">Número do CPF, opcional.</param>
    /// <param name="motherName">Nome completo da mãe, opcional.</param>
    /// <param name="gender">Sexo, opcional.</param>
    /// <param name="raceColor">Cor/raça, opcional.</param>
    /// <param name="phone">Telefone de contato, opcional.</param>
    /// <param name="email">E-mail de contato, opcional.</param>
    /// <param name="address">Endereço residencial, opcional.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso, ou de falha quando a data de
    /// nascimento for futura ou anterior a 1900.
    /// </returns>
    public static Result<Person> Create(
        PersonName name,
        DateOnly birthDate,
        DateOnly today,
        Cns? cns = null,
        Cpf? cpf = null,
        string? motherName = null,
        string? gender = null,
        string? raceColor = null,
        PhoneNumber? phone = null,
        EmailAddress? email = null,
        Address? address = null)
    {
        if (birthDate > today)
            return Result<Person>.Failure(PersonErrors.DataNascimentoFutura);

        if (birthDate < new DateOnly(1900, 1, 1))
            return Result<Person>.Failure(PersonErrors.DataNascimentoMuitoAntiga);

        var person = new Person(
            Guid.NewGuid(), name, birthDate, cns, cpf, motherName?.Trim(), gender?.Trim(), raceColor?.Trim(),
            phone, email, address, DateTime.UtcNow);

        person.Raise(new PersonRegisteredEvent(person.Id));

        return Result<Person>.Success(person);
    }

    /// <summary>
    /// Atualiza os dados básicos de contato e identificação da pessoa.
    /// </summary>
    /// <param name="motherName">Novo nome da mãe, opcional.</param>
    /// <param name="gender">Novo sexo, opcional.</param>
    /// <param name="raceColor">Nova cor/raça, opcional.</param>
    /// <param name="phone">Novo telefone, opcional.</param>
    /// <param name="email">Novo e-mail, opcional.</param>
    /// <param name="address">Novo endereço, opcional.</param>
    /// <returns>Um <see cref="Result"/> sempre bem-sucedido.</returns>
    public Result UpdateBasicData(
        string? motherName,
        string? gender,
        string? raceColor,
        PhoneNumber? phone,
        EmailAddress? email,
        Address? address)
    {
        MotherName = motherName?.Trim();
        Gender = gender?.Trim();
        RaceColor = raceColor?.Trim();
        Phone = phone;
        Email = email;
        Address = address;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Atualiza o perfil educacional/escolar da pessoa (NAPE A.2/A.3/A.4).
    /// Todos os campos são independentes do cadastro básico porque são
    /// preenchidos por serviços diferentes (saúde vs. educação) em momentos
    /// diferentes do fluxo.
    /// </summary>
    /// <param name="naturality">Naturalidade, opcional.</param>
    /// <param name="currentSchool">Escola atual, opcional.</param>
    /// <param name="grade">Série escolar, opcional.</param>
    /// <param name="shift">Turno escolar, opcional.</param>
    /// <param name="classGroup">Turma escolar, opcional.</param>
    /// <param name="zone">Zona de residência, opcional.</param>
    /// <param name="schoolEnrollment">Matrícula escolar, opcional.</param>
    /// <param name="referredBySchool">Se o encaminhamento partiu da escola, opcional.</param>
    /// <param name="disabilityTypes">Tipos de deficiência, separados por vírgula (RN32), opcional.</param>
    /// <param name="needsSpecialEducation">Se necessita de AEE, opcional.</param>
    /// <param name="attendsTutoring">Se frequenta reforço escolar, opcional.</param>
    /// <param name="hasFailedGrade">Se já repetiu alguma série, opcional.</param>
    /// <returns>Um <see cref="Result"/> sempre bem-sucedido.</returns>
    public Result UpdateEducationalProfile(
        string? naturality,
        string? currentSchool,
        string? grade,
        string? shift,
        string? classGroup,
        string? zone,
        string? schoolEnrollment,
        bool? referredBySchool,
        string? disabilityTypes,
        bool? needsSpecialEducation,
        bool? attendsTutoring,
        bool? hasFailedGrade)
    {
        Naturality = naturality?.Trim();
        CurrentSchool = currentSchool?.Trim();
        Grade = grade?.Trim();
        Shift = shift?.Trim();
        ClassGroup = classGroup?.Trim();
        Zone = zone?.Trim();
        SchoolEnrollment = schoolEnrollment?.Trim();
        ReferredBySchool = referredBySchool;
        DisabilityTypes = disabilityTypes?.Trim();
        NeedsSpecialEducation = needsSpecialEducation;
        AttendsTutoring = attendsTutoring;
        HasFailedGrade = hasFailedGrade;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Vincula um novo responsável legal a esta pessoa.
    /// </summary>
    /// <param name="name">Nome completo do responsável.</param>
    /// <param name="relationship">Grau de parentesco com a pessoa.</param>
    /// <param name="cns">CNS do responsável, opcional.</param>
    /// <param name="birthDate">Data de nascimento do responsável, opcional.</param>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha quando o relacionamento
    /// não for informado.
    /// </returns>
    public Result AddGuardian(PersonName name, string? relationship, Cns? cns = null, DateOnly? birthDate = null)
    {
        var guardianResult = Guardian.Create(Id, name, relationship, cns, birthDate);
        if (guardianResult.IsFailure)
            return Result.Failure(guardianResult.Error);

        _guardians.Add(guardianResult.Value);
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// Remove um responsável legal vinculado a esta pessoa.
    /// </summary>
    /// <param name="guardianId">Identificador do responsável a ser removido.</param>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="PersonErrors.ResponsavelNaoEncontrado"/> quando o
    /// responsável não existir.
    /// </returns>
    public Result RemoveGuardian(Guid guardianId)
    {
        var guardian = _guardians.FirstOrDefault(g => g.Id == guardianId);
        if (guardian is null)
            return Result.Failure(PersonErrors.ResponsavelNaoEncontrado);

        _guardians.Remove(guardian);
        UpdatedAt = DateTime.UtcNow;
        return Result.Success();
    }

    /// <summary>
    /// Calcula a idade atual da pessoa, em anos completos.
    /// </summary>
    /// <param name="today">Data de referência para o cálculo, injetada para testabilidade.</param>
    /// <returns>Idade em anos completos na data de referência.</returns>
    public int CalculateAge(DateOnly today)
    {
        var age = today.Year - BirthDate.Year;
        if (BirthDate > today.AddYears(-age))
            age--;

        return age;
    }

    /// <summary>Indica se a pessoa possui um CNS cadastrado.</summary>
    /// <returns><see langword="true"/> quando <see cref="Cns"/> não é <see langword="null"/>.</returns>
    public bool HasValidCns() => Cns is not null;

    /// <summary>
    /// Gera uma chave demográfica (nome normalizado + data de nascimento)
    /// usada como base para a busca de candidatos a duplicidade de cadastro
    /// (ver <see cref="Entities.DuplicateAlert"/> e a extensão <c>pg_trgm</c>
    /// na camada de infraestrutura).
    /// </summary>
    /// <returns>Chave demográfica no formato "nome-normalizado|aaaa-mm-dd".</returns>
    public string DemographicKey() => $"{Name.Normalized}|{BirthDate:yyyy-MM-dd}";
}
