namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros do módulo de Marcos de Desenvolvimento (<c>DevelopmentMilestones</c>).</summary>
public static class DevelopmentMilestonesErrors
{
    /// <summary>Marcos de desenvolvimento já cadastrados para esta pessoa.</summary>
    public static readonly Error JaCadastrados = new("DEV_001", "Marcos de desenvolvimento já cadastrados para esta pessoa.", ErrorType.Conflict);

    /// <summary>Idade inválida (deve estar entre 0 e 120 meses).</summary>
    public static readonly Error IdadeInvalida = new("DEV_002", "Idade inválida (deve estar entre 0 e 120 meses).");

    /// <summary>Dominância manual inválida.</summary>
    public static readonly Error DominanciaManualInvalida = new("DEV_003", "Dominância manual inválida (aceitos: Destro, Canhoto, Ambidestro).");
}
