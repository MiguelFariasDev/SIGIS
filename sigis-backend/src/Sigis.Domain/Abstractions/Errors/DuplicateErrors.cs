namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros do módulo de Deduplicação (<c>DuplicateAlert</c>).</summary>
public static class DuplicateErrors
{
    /// <summary>Score de similaridade deve estar entre 0 e 1.</summary>
    public static readonly Error ScoreInvalido = new("DUPLICATE_001", "Score de similaridade deve estar entre 0 e 1.");

    /// <summary>Score abaixo do limiar mínimo (0,75).</summary>
    public static readonly Error ScoreAbaixoLimiar = new("DUPLICATE_002", "Score abaixo do limiar mínimo (0,75).");

    /// <summary>Não é possível criar alerta de duplicidade para a mesma pessoa.</summary>
    public static readonly Error MesmaPessoa = new("DUPLICATE_003", "Não é possível criar alerta de duplicidade para a mesma pessoa.");

    /// <summary>Alerta de duplicidade já foi resolvido.</summary>
    public static readonly Error AlertaJaResolvido = new("DUPLICATE_004", "Alerta de duplicidade já foi resolvido.", ErrorType.Conflict);
}
