namespace Sigis.Domain.Enums;

/// <summary>
/// Dia da semana de um atendimento concomitante recorrente.
/// </summary>
/// <remarks>
/// Nome igual ao de <see cref="System.DayOfWeek"/> por exigência da
/// especificação de domínio — em arquivos que também usam
/// <see cref="System.DayOfWeek"/>, referencie este tipo com o alias
/// <c>using DayOfWeek = Sigis.Domain.Enums.DayOfWeek;</c> para evitar
/// ambiguidade (CS0104).
/// </remarks>
public enum DayOfWeek
{
    /// <summary>Segunda-feira.</summary>
    Monday = 1,

    /// <summary>Terça-feira.</summary>
    Tuesday = 2,

    /// <summary>Quarta-feira.</summary>
    Wednesday = 3,

    /// <summary>Quinta-feira.</summary>
    Thursday = 4,

    /// <summary>Sexta-feira.</summary>
    Friday = 5,

    /// <summary>Sábado.</summary>
    Saturday = 6,

    /// <summary>Domingo.</summary>
    Sunday = 7
}
