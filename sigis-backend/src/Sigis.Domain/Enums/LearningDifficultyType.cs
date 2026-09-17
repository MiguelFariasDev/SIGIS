namespace Sigis.Domain.Enums;

/// <summary>Tipo de dificuldade de aprendizagem identificada na sondagem psicopedagógica (NAPE A.3).</summary>
public enum LearningDifficultyType
{
    /// <summary>Dificuldade de leitura.</summary>
    Leitura = 1,

    /// <summary>Dificuldade de escrita.</summary>
    Escrita = 2,

    /// <summary>Dificuldade de cálculo.</summary>
    Calculo = 3,

    /// <summary>Dificuldade de atenção.</summary>
    Atencao = 4,

    /// <summary>Dificuldade de concentração.</summary>
    Concentracao = 5,

    /// <summary>Dificuldade de coordenação motora.</summary>
    CoordenacaoMotora = 6,

    /// <summary>Outro tipo de dificuldade não listado.</summary>
    Outro = 99
}
