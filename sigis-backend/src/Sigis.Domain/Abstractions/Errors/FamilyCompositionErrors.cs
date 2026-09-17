namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros do módulo de Composição Familiar (<c>FamilyComposition</c>).</summary>
public static class FamilyCompositionErrors
{
    /// <summary>Composição familiar já cadastrada para esta pessoa.</summary>
    public static readonly Error JaCadastrada = new("FAMILY_001", "Composição familiar já cadastrada para esta pessoa.");

    /// <summary>Número de abortos não pode ser maior que número de gestações.</summary>
    public static readonly Error AbortosMaiorQueGestacoes = new("FAMILY_002", "Número de abortos não pode ser maior que número de gestações.");

    /// <summary>Tipo de filiação inválido (aceitos: Natural, Adotivo).</summary>
    public static readonly Error TipoFiliacaoInvalido = new("FAMILY_003", "Tipo de filiação inválido (aceitos: Natural, Adotivo).");

    /// <summary>Tipo de parto inválido (aceitos: Normal, Cesarea).</summary>
    public static readonly Error TipoPartoInvalido = new("FAMILY_004", "Tipo de parto inválido (aceitos: Normal, Cesarea).");

    /// <summary>Número de gestações não pode ser negativo.</summary>
    public static readonly Error GestacoesNegativas = new("FAMILY_005", "Número de gestações não pode ser negativo.");

    /// <summary>Número de abortos não pode ser negativo.</summary>
    public static readonly Error AbortosNegativos = new("FAMILY_006", "Número de abortos não pode ser negativo.");

    /// <summary>Número de irmãos não pode ser negativo (RN-FC06).</summary>
    public static readonly Error IrmaosNegativo = new("FAMILY_007", "Número de irmãos não pode ser negativo.");

    /// <summary>Número de pessoas na residência não pode ser negativo (RN-FC05).</summary>
    public static readonly Error MembrosResidenciaNegativo = new("FAMILY_008", "Número de pessoas na residência não pode ser negativo.");
}
