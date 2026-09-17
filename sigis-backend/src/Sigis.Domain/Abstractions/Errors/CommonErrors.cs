namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros genéricos, não associados a um módulo de domínio específico.</summary>
public static class CommonErrors
{
    /// <summary>Erro interno inesperado.</summary>
    public static readonly Error InternalError = new("COMMON_001", "Erro interno inesperado.");

    /// <summary>Recurso não encontrado.</summary>
    public static readonly Error NotFound = new("COMMON_002", "Recurso não encontrado.");

    /// <summary>Acesso não autorizado.</summary>
    public static readonly Error Unauthorized = new("COMMON_003", "Acesso não autorizado.");
}
