using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Enums;

namespace Sigis.Domain.Entities;

/// <summary>
/// Consentimento granular concedido por uma pessoa (ou seu responsável) para
/// tratamento de dados por uma finalidade específica (LGPD, art. 8º e
/// art. 11, I). Relação 1:N com <see cref="Person"/> — cada revogação gera
/// um registro histórico; um novo consentimento do mesmo tipo é um novo
/// registro (RN-PC04), nunca uma reativação do anterior.
/// </summary>
public sealed class PersonConsent : Entity
{
    /// <summary>Identificador da pessoa.</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Tipo/finalidade do consentimento.</summary>
    public ConsentType Type { get; private set; }

    /// <summary>Indica se o consentimento foi concedido.</summary>
    public bool Granted { get; private set; }

    /// <summary>Data e hora (UTC) da concessão.</summary>
    public DateTime GrantedAt { get; private set; }

    /// <summary>Identificador do responsável que concedeu o consentimento, quando aplicável.</summary>
    public Guid? GrantedByGuardianId { get; private set; }

    /// <summary>Data e hora (UTC) da revogação, quando revogado.</summary>
    public DateTime? RevokedAt { get; private set; }

    /// <summary>Versão do termo de consentimento aceito.</summary>
    public string Version { get; private set; }

    /// <summary>Evidência da concessão (ex.: "presencial em unidade X"), quando informada.</summary>
    public string? Evidence { get; private set; }

    /// <summary>Data e hora (UTC) de criação do registro.</summary>
    public DateTime CreatedAt { get; private set; }

    private PersonConsent(
        Guid id, Guid personId, ConsentType type, bool granted, DateTime grantedAt,
        Guid? grantedByGuardianId, string version, string? evidence, DateTime createdAt)
    {
        Id = id;
        PersonId = personId;
        Type = type;
        Granted = granted;
        GrantedAt = grantedAt;
        GrantedByGuardianId = grantedByGuardianId;
        Version = version;
        Evidence = evidence;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Registra um novo consentimento concedido para a pessoa.
    /// </summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="type">Tipo/finalidade do consentimento.</param>
    /// <param name="grantedAt">Data e hora (UTC) da concessão.</param>
    /// <param name="version">Versão do termo aceito.</param>
    /// <param name="createdAt">Data e hora (UTC) de criação do registro.</param>
    /// <param name="grantedByGuardianId">Identificador do responsável que concedeu, quando aplicável.</param>
    /// <param name="evidence">Evidência da concessão, opcional.</param>
    /// <returns>Um <see cref="Result{T}"/> sempre bem-sucedido com o consentimento concedido.</returns>
    /// <remarks>
    /// RN-PC01 (um único consentimento ativo por tipo) é reforçada pelo
    /// índice único parcial <c>(person_id, type) WHERE revoked_at IS
    /// NULL</c> no banco. RN-PC03 (consentimento clínico obrigatório no
    /// cadastro) é responsabilidade da camada de aplicação, que decide
    /// quantos e quais consentimentos exigir no fluxo de cadastro.
    /// </remarks>
    public static Result<PersonConsent> Create(
        Guid personId,
        ConsentType type,
        DateTime grantedAt,
        string version,
        DateTime createdAt,
        Guid? grantedByGuardianId = null,
        string? evidence = null)
    {
        return Result<PersonConsent>.Success(new PersonConsent(
            Guid.NewGuid(), personId, type, granted: true, grantedAt, grantedByGuardianId,
            version.Trim(), evidence?.Trim(), createdAt));
    }

    /// <summary>Indica se este consentimento ainda está ativo (não revogado).</summary>
    /// <returns><see langword="true"/> quando <see cref="RevokedAt"/> é <see langword="null"/>.</returns>
    public bool IsActive() => RevokedAt is null;

    /// <summary>
    /// Revoga o consentimento.
    /// </summary>
    /// <param name="revokedAt">Data e hora (UTC) da revogação.</param>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="PersonConsentErrors.RevogadoNaoPodeReativar"/> quando já
    /// revogado, ou com <see cref="PersonConsentErrors.RevogacaoAnteriorAConcessao"/>
    /// quando a data de revogação for anterior à concessão.
    /// </returns>
    public Result Revoke(DateTime revokedAt)
    {
        if (!IsActive())
            return Result.Failure(PersonConsentErrors.RevogadoNaoPodeReativar);

        if (revokedAt < GrantedAt)
            return Result.Failure(PersonConsentErrors.RevogacaoAnteriorAConcessao);

        RevokedAt = revokedAt;
        return Result.Success();
    }
}
