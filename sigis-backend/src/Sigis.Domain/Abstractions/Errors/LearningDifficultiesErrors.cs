namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros do módulo de Dificuldades de Aprendizagem (<c>LearningDifficulties</c>).</summary>
public static class LearningDifficultiesErrors
{
    /// <summary>Já existe registro desta dificuldade para a pessoa.</summary>
    public static readonly Error RegistroDuplicado = new("LEARNING_001", "Já existe registro desta dificuldade para a pessoa.", ErrorType.Conflict);

    /// <summary>Tipo de dificuldade inválido.</summary>
    public static readonly Error TipoInvalido = new("LEARNING_002", "Tipo de dificuldade inválido.");

    /// <summary>Gravidade inválida.</summary>
    public static readonly Error GravidadeInvalida = new("LEARNING_003", "Gravidade inválida (aceitos: Leve, Moderada, Severa).");

    /// <summary>Data da sondagem não pode ser futura.</summary>
    public static readonly Error DataSondagemFutura = new("LEARNING_004", "Data da sondagem não pode ser futura.");
}
