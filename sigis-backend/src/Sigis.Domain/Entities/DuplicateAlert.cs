using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Enums;
using Sigis.Domain.Events;

namespace Sigis.Domain.Entities;

/// <summary>
/// Alerta de possível duplicidade de cadastro entre duas pessoas, gerado
/// pelo processo de deduplicação em 3 camadas (CNS exato, CPF exato,
/// probabilístico via <c>pg_trgm</c>). Agregado raiz do módulo de
/// Deduplicação.
/// </summary>
/// <remarks>
/// Regra crítica do domínio: cadastros nunca são fundidos automaticamente.
/// Todo alerta permanece <see cref="DuplicateAlertStatus.Pending"/> até que
/// um coordenador confirme a mesclagem (<see cref="ConfirmMerge"/>) ou marque
/// como falso positivo (<see cref="MarkAsFalsePositive"/>).
/// </remarks>
public sealed class DuplicateAlert : AggregateRoot
{
    /// <summary>Limiar mínimo de similaridade para justificar um alerta (RN02).</summary>
    public const double MinimumSimilarityThreshold = 0.75;

    /// <summary>Identificador do primeiro cadastro envolvido no alerta.</summary>
    public Guid PersonId1 { get; private set; }

    /// <summary>Identificador do segundo cadastro envolvido no alerta.</summary>
    public Guid PersonId2 { get; private set; }

    /// <summary>Score de similaridade entre os dois cadastros, entre 0 e 1.</summary>
    public double SimilarityScore { get; private set; }

    /// <summary>Situação atual do alerta.</summary>
    public DuplicateAlertStatus Status { get; private set; }

    /// <summary>Identificador do profissional que resolveu o alerta, quando já resolvido.</summary>
    public Guid? ResolvedByProfessionalId { get; private set; }

    /// <summary>Data e hora (UTC) em que o alerta foi resolvido, quando já resolvido.</summary>
    public DateTime? ResolvedAt { get; private set; }

    /// <summary>Data e hora (UTC) de criação do alerta.</summary>
    public DateTime CreatedAt { get; private set; }

    private DuplicateAlert(Guid id, Guid personId1, Guid personId2, double similarityScore, DateTime createdAt)
    {
        Id = id;
        PersonId1 = personId1;
        PersonId2 = personId2;
        SimilarityScore = similarityScore;
        Status = DuplicateAlertStatus.Pending;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Cria um novo alerta de duplicidade, com situação inicial
    /// <see cref="DuplicateAlertStatus.Pending"/>, emitindo
    /// <see cref="DuplicateDetectedEvent"/>.
    /// </summary>
    /// <param name="personId1">Identificador do primeiro cadastro envolvido.</param>
    /// <param name="personId2">Identificador do segundo cadastro envolvido.</param>
    /// <param name="similarityScore">Score de similaridade calculado entre os dois cadastros (0 a 1).</param>
    /// <param name="createdAt">Data e hora (UTC) de criação do alerta.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso, ou de falha quando os dois
    /// identificadores forem iguais, o score estiver fora do intervalo
    /// [0, 1] (RN02), ou o score estiver abaixo do limiar mínimo (RN02).
    /// </returns>
    public static Result<DuplicateAlert> Create(
        Guid personId1, Guid personId2, double similarityScore, DateTime createdAt)
    {
        if (personId1 == personId2)
            return Result<DuplicateAlert>.Failure(DuplicateErrors.MesmaPessoa);

        if (similarityScore is < 0 or > 1)
            return Result<DuplicateAlert>.Failure(DuplicateErrors.ScoreInvalido);

        if (similarityScore < MinimumSimilarityThreshold)
            return Result<DuplicateAlert>.Failure(DuplicateErrors.ScoreAbaixoLimiar);

        var alert = new DuplicateAlert(Guid.NewGuid(), personId1, personId2, similarityScore, createdAt);
        alert.Raise(new DuplicateDetectedEvent(personId1, personId2, similarityScore));

        return Result<DuplicateAlert>.Success(alert);
    }

    /// <summary>Indica se o alerta ainda aguarda revisão de um coordenador.</summary>
    /// <returns><see langword="true"/> quando <see cref="Status"/> é <see cref="DuplicateAlertStatus.Pending"/>.</returns>
    public bool IsPending() => Status == DuplicateAlertStatus.Pending;

    /// <summary>
    /// Confirma que os dois cadastros são da mesma pessoa e devem ser
    /// mesclados, transicionando para <see cref="DuplicateAlertStatus.Merged"/>.
    /// </summary>
    /// <param name="professionalId">
    /// Identificador do profissional (coordenador) que resolveu o alerta.
    /// </param>
    /// <param name="resolvedAt">Data e hora (UTC) da resolução.</param>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="DuplicateErrors.AlertaJaResolvido"/> quando o alerta já
    /// tiver sido resolvido anteriormente.
    /// </returns>
    /// <remarks>
    /// A mesclagem efetiva do histórico dos dois cadastros sob um
    /// identificador canônico é responsabilidade da camada de aplicação —
    /// este método apenas registra a decisão do coordenador.
    /// </remarks>
    public Result ConfirmMerge(Guid professionalId, DateTime resolvedAt)
    {
        if (!IsPending())
            return Result.Failure(DuplicateErrors.AlertaJaResolvido);

        Status = DuplicateAlertStatus.Merged;
        ResolvedByProfessionalId = professionalId;
        ResolvedAt = resolvedAt;
        return Result.Success();
    }

    /// <summary>
    /// Marca o alerta como falso positivo — os dois cadastros são de pessoas
    /// diferentes e permanecem separados.
    /// </summary>
    /// <param name="professionalId">
    /// Identificador do profissional (coordenador) que resolveu o alerta.
    /// </param>
    /// <param name="resolvedAt">Data e hora (UTC) da resolução.</param>
    /// <returns>
    /// Um <see cref="Result"/> de sucesso, ou de falha com
    /// <see cref="DuplicateErrors.AlertaJaResolvido"/> quando o alerta já
    /// tiver sido resolvido anteriormente.
    /// </returns>
    public Result MarkAsFalsePositive(Guid professionalId, DateTime resolvedAt)
    {
        if (!IsPending())
            return Result.Failure(DuplicateErrors.AlertaJaResolvido);

        Status = DuplicateAlertStatus.FalsePositive;
        ResolvedByProfessionalId = professionalId;
        ResolvedAt = resolvedAt;
        return Result.Success();
    }
}
