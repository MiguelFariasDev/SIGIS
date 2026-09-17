namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros do módulo de Auditoria de Acesso (<c>AccessLog</c>).</summary>
public static class AccessLogErrors
{
    /// <summary>Base legal é obrigatória.</summary>
    public static readonly Error BaseLegalObrigatoria = new("ACCESS_001", "Base legal é obrigatória.");

    /// <summary>Justificativa é obrigatória para acesso entre unidades.</summary>
    public static readonly Error JustificativaObrigatoriaCrossUnidade = new("ACCESS_002", "Justificativa é obrigatória para acesso entre unidades.");

    /// <summary>Ação de auditoria inválida.</summary>
    public static readonly Error AcaoInvalida = new("ACCESS_003", "Ação de auditoria inválida.");
}
