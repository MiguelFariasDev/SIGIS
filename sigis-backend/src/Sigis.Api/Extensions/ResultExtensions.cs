using Microsoft.AspNetCore.Mvc;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Extensions;

/// <summary>
/// Converte um <see cref="Error"/> de domínio no <see cref="ObjectResult"/>
/// HTTP correspondente, a partir de <see cref="Error.Type"/> — único ponto
/// da API que traduz <see cref="ErrorType"/> em código de status, para que
/// nenhuma controller precise conhecer os códigos de erro de cada módulo.
/// </summary>
public static class ResultExtensions
{
    /// <summary>Converte o erro em uma resposta HTTP com o corpo do erro e o status correspondente ao seu <see cref="ErrorType"/>.</summary>
    /// <param name="error">Erro de domínio a converter.</param>
    /// <returns>Um <see cref="ObjectResult"/> com o status HTTP mapeado a partir de <see cref="Error.Type"/>.</returns>
    public static ObjectResult ToErrorResult(this Error error)
    {
        var statusCode = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            ErrorType.Internal => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest,
        };

        return new ObjectResult(error) { StatusCode = statusCode };
    }
}
