using Microsoft.AspNetCore.Mvc;
using Sigis.Api.Extensions;
using Sigis.Domain.Abstractions;

namespace Sigis.Api.Controllers;

/// <summary>
/// Classe base para as controllers do SIGIS: fixa o roteamento padrão
/// (<c>api/[controller]</c>) e concentra a conversão de <see cref="Result"/>/
/// <see cref="Result{T}"/> dos casos de uso (MediatR) em <see cref="IActionResult"/>,
/// para que nenhuma controller precise montar códigos de status na mão.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Converte um <see cref="Result{T}"/> em uma resposta HTTP: 200 OK com o
    /// valor em caso de sucesso, ou o status mapeado a partir de <see cref="Error.Type"/> em caso de falha.
    /// </summary>
    /// <typeparam name="T">Tipo do valor retornado pelo caso de uso.</typeparam>
    /// <param name="result">Resultado do caso de uso.</param>
    protected IActionResult ToHttpResult<T>(Result<T> result) =>
        result.IsSuccess ? Ok(result.Value) : result.Error.ToErrorResult();

    /// <summary>
    /// Converte um <see cref="Result{T}"/> em uma resposta HTTP: 201 Created
    /// com o valor em caso de sucesso, ou o status mapeado a partir de <see cref="Error.Type"/> em caso de falha.
    /// Usado por ações de criação (POST).
    /// </summary>
    /// <typeparam name="T">Tipo do valor retornado pelo caso de uso.</typeparam>
    /// <param name="result">Resultado do caso de uso.</param>
    protected IActionResult ToCreatedResult<T>(Result<T> result) =>
        result.IsSuccess ? StatusCode(StatusCodes.Status201Created, result.Value) : result.Error.ToErrorResult();

    /// <summary>
    /// Converte um <see cref="Result"/> sem valor em uma resposta HTTP: 204 No
    /// Content em caso de sucesso, ou o status mapeado a partir de <see cref="Error.Type"/> em caso de falha.
    /// Usado por ações que não retornam corpo (DELETE, ações de estado).
    /// </summary>
    /// <param name="result">Resultado do caso de uso.</param>
    protected IActionResult ToHttpResult(Result result) =>
        result.IsSuccess ? NoContent() : result.Error.ToErrorResult();
}
