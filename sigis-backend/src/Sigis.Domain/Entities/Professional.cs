using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Enums;
using Sigis.Domain.ValueObjects;

namespace Sigis.Domain.Entities;

/// <summary>
/// Profissional de atendimento vinculado a uma unidade de serviço da rede.
/// </summary>
public sealed class Professional : Entity
{
    /// <summary>Nome completo do profissional.</summary>
    public PersonName Name { get; private set; }

    /// <summary>Especialidade do profissional (ex.: "Psicólogo", "Psicopedagogo").</summary>
    public string Specialty { get; private set; }

    /// <summary>Identificador da unidade de serviço à qual o profissional está vinculado.</summary>
    public Guid UnitId { get; private set; }

    /// <summary>Papel de controle de acesso (RBAC) do profissional.</summary>
    public RbacRole Role { get; private set; }

    /// <summary>Indica se o profissional está ativo no sistema.</summary>
    public bool Active { get; private set; }

    /// <summary>E-mail de acesso do profissional (usado como login), único no sistema.</summary>
    public EmailAddress Email { get; private set; }

    /// <summary>Hash da senha do profissional (BCrypt) — nunca a senha em claro.</summary>
    public string PasswordHash { get; private set; }

    /// <summary>Data e hora (UTC) do último login bem-sucedido, quando já houve login.</summary>
    public DateTime? LastLoginAt { get; private set; }

    private Professional(
        Guid id, PersonName name, string specialty, Guid unitId, RbacRole role, bool active,
        EmailAddress email, string passwordHash)
    {
        Id = id;
        Name = name;
        Specialty = specialty;
        UnitId = unitId;
        Role = role;
        Active = active;
        Email = email;
        PasswordHash = passwordHash;
    }

    /// <summary>
    /// Cria um novo <see cref="Professional"/>, ativo por padrão.
    /// </summary>
    /// <param name="name">Nome completo do profissional, já validado como <see cref="PersonName"/>.</param>
    /// <param name="specialty">Especialidade do profissional.</param>
    /// <param name="unitId">Identificador da unidade de serviço.</param>
    /// <param name="role">Papel de controle de acesso (RBAC).</param>
    /// <param name="email">E-mail de acesso (login), já validado como <see cref="EmailAddress"/>.</param>
    /// <param name="passwordHash">
    /// Hash da senha (gerado por <c>IPasswordHasher</c> na camada de
    /// aplicação) — a entidade nunca recebe nem manipula a senha em claro.
    /// </param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso, ou de falha quando especialidade
    /// ou unidade não forem informadas.
    /// </returns>
    public static Result<Professional> Create(
        PersonName name, string? specialty, Guid unitId, RbacRole role, EmailAddress email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(specialty))
            return Result<Professional>.Failure(PersonErrors.EspecialidadeProfissionalObrigatoria);

        if (unitId == Guid.Empty)
            return Result<Professional>.Failure(PersonErrors.UnidadeProfissionalObrigatoria);

        return Result<Professional>.Success(
            new Professional(Guid.NewGuid(), name, specialty.Trim(), unitId, role, active: true, email, passwordHash));
    }

    /// <summary>Substitui o hash de senha do profissional (ex.: troca de senha).</summary>
    /// <param name="passwordHash">Novo hash de senha, já gerado por <c>IPasswordHasher</c>.</param>
    /// <returns>Um <see cref="Result"/> sempre bem-sucedido.</returns>
    public Result SetPassword(string passwordHash)
    {
        PasswordHash = passwordHash;
        return Result.Success();
    }

    /// <summary>Registra o instante do login bem-sucedido mais recente.</summary>
    /// <param name="dateTime">Data e hora (UTC) do login.</param>
    /// <returns>Um <see cref="Result"/> sempre bem-sucedido.</returns>
    public Result RegisterLogin(DateTime dateTime)
    {
        LastLoginAt = dateTime;
        return Result.Success();
    }

    /// <summary>Ativa o profissional, permitindo que volte a operar no sistema.</summary>
    /// <returns>Um <see cref="Result"/> sempre bem-sucedido.</returns>
    public Result Activate()
    {
        Active = true;
        return Result.Success();
    }

    /// <summary>Desativa o profissional, impedindo novas operações em seu nome.</summary>
    /// <returns>Um <see cref="Result"/> sempre bem-sucedido.</returns>
    public Result Deactivate()
    {
        Active = false;
        return Result.Success();
    }

    /// <summary>
    /// Indica se o profissional pode resolver alertas de duplicidade
    /// (mesclar cadastros ou marcar como falso positivo).
    /// </summary>
    /// <returns><see langword="true"/> apenas para o papel <see cref="RbacRole.Coordinator"/>.</returns>
    public bool CanResolveDuplicates() => Role == RbacRole.Coordinator;

    /// <summary>
    /// Indica se o profissional pode visualizar dados consolidados de toda a
    /// rede (não apenas da própria unidade) — RN06.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> para os papéis <see cref="RbacRole.Coordinator"/>
    /// ou <see cref="RbacRole.Auditor"/>.
    /// </returns>
    public bool CanSeeFullNetwork() => Role is RbacRole.Coordinator or RbacRole.Auditor;

    /// <summary>
    /// Indica se o profissional pode consultar os logs de acesso e
    /// compartilhamento (auditoria) — RN07.
    /// </summary>
    /// <returns><see langword="true"/> apenas para o papel <see cref="RbacRole.Auditor"/>.</returns>
    public bool CanSeeLogs() => Role == RbacRole.Auditor;
}
