namespace Sigis.Domain.Abstractions;

/// <summary>
/// Categoria de um <see cref="Error"/>, usada pela camada HTTP (<c>Sigis.Api</c>)
/// para converter um <see cref="Result"/>/<see cref="Result{T}"/> de falha no
/// código de status correto, sem que cada controller precise conhecer os
/// códigos de erro de cada módulo.
/// </summary>
public enum ErrorType
{
    /// <summary>Falha de validação ou regra de negócio — HTTP 400.</summary>
    Validation,

    /// <summary>Recurso solicitado não existe — HTTP 404.</summary>
    NotFound,

    /// <summary>Estado atual do recurso não permite a operação, ou duplicidade — HTTP 409.</summary>
    Conflict,

    /// <summary>Credenciais ausentes ou inválidas — HTTP 401.</summary>
    Unauthorized,

    /// <summary>Autenticado, mas sem permissão para a ação — HTTP 403.</summary>
    Forbidden,

    /// <summary>Erro interno inesperado — HTTP 500.</summary>
    Internal,
}
