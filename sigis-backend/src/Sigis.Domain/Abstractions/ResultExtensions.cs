namespace Sigis.Domain.Abstractions;

/// <summary>
/// Métodos de extensão para composição funcional de <see cref="Result"/> e
/// <see cref="Result{T}"/>, evitando encadeamentos manuais de verificação de
/// <see cref="Result.IsSuccess"/>.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Encadeia uma nova operação que também retorna um <see cref="Result{TOut}"/>,
    /// executando-a apenas quando o resultado de origem for sucesso.
    /// </summary>
    /// <typeparam name="TIn">Tipo do valor do resultado de origem.</typeparam>
    /// <typeparam name="TOut">Tipo do valor do resultado produzido pela próxima operação.</typeparam>
    /// <param name="result">Resultado de origem.</param>
    /// <param name="next">Função a ser executada com o valor de origem, em caso de sucesso.</param>
    /// <returns>
    /// O resultado produzido por <paramref name="next"/> quando <paramref name="result"/>
    /// for sucesso; caso contrário, um <see cref="Result{TOut}"/> de falha com o mesmo erro.
    /// </returns>
    public static Result<TOut> Bind<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> next)
        => result.IsSuccess ? next(result.Value) : Result<TOut>.Failure(result.Error);

    /// <summary>
    /// Transforma o valor de um resultado de sucesso, preservando a falha quando houver.
    /// </summary>
    /// <typeparam name="TIn">Tipo do valor do resultado de origem.</typeparam>
    /// <typeparam name="TOut">Tipo do valor produzido pela transformação.</typeparam>
    /// <param name="result">Resultado de origem.</param>
    /// <param name="map">Função de transformação aplicada ao valor, em caso de sucesso.</param>
    /// <returns>
    /// Um <see cref="Result{TOut}"/> de sucesso com o valor transformado, ou de
    /// falha com o mesmo erro de <paramref name="result"/>.
    /// </returns>
    public static Result<TOut> Map<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> map)
        => result.IsSuccess ? Result<TOut>.Success(map(result.Value)) : Result<TOut>.Failure(result.Error);

    /// <summary>
    /// Executa uma ação de efeito colateral com o valor do resultado, apenas em
    /// caso de sucesso, sem alterar o resultado original.
    /// </summary>
    /// <typeparam name="T">Tipo do valor do resultado.</typeparam>
    /// <param name="result">Resultado a ser observado.</param>
    /// <param name="action">Ação executada com o valor, em caso de sucesso.</param>
    /// <returns>O próprio <paramref name="result"/>, inalterado.</returns>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
            action(result.Value);

        return result;
    }

    /// <summary>
    /// Executa uma ação de efeito colateral apenas quando o resultado for sucesso.
    /// </summary>
    /// <param name="result">Resultado a ser observado.</param>
    /// <param name="action">Ação executada em caso de sucesso.</param>
    /// <returns>O próprio <paramref name="result"/>, inalterado.</returns>
    public static Result OnSuccess(this Result result, Action action)
    {
        if (result.IsSuccess)
            action();

        return result;
    }

    /// <summary>
    /// Executa uma ação de efeito colateral com o erro, apenas quando o resultado for falha.
    /// </summary>
    /// <param name="result">Resultado a ser observado.</param>
    /// <param name="action">Ação executada com o erro, em caso de falha.</param>
    /// <returns>O próprio <paramref name="result"/>, inalterado.</returns>
    public static Result OnFailure(this Result result, Action<Error> action)
    {
        if (result.IsFailure)
            action(result.Error);

        return result;
    }

    /// <summary>
    /// Garante que o valor de um resultado de sucesso satisfaça um predicado,
    /// convertendo-o em falha com o erro informado caso não satisfaça.
    /// </summary>
    /// <typeparam name="T">Tipo do valor do resultado.</typeparam>
    /// <param name="result">Resultado a ser validado.</param>
    /// <param name="predicate">Condição que o valor deve satisfazer.</param>
    /// <param name="error">Erro retornado quando o predicado não for satisfeito.</param>
    /// <returns>
    /// O próprio <paramref name="result"/> quando já é falha ou quando o
    /// predicado é satisfeito; caso contrário, um <see cref="Result{T}"/> de
    /// falha com <paramref name="error"/>.
    /// </returns>
    public static Result<T> Ensure<T>(this Result<T> result, Func<T, bool> predicate, Error error)
    {
        if (result.IsFailure)
            return result;

        return predicate(result.Value) ? result : Result<T>.Failure(error);
    }

    /// <summary>
    /// Encapsula um valor em um <see cref="Result{T}"/> de sucesso.
    /// </summary>
    /// <typeparam name="T">Tipo do valor.</typeparam>
    /// <param name="value">Valor a ser encapsulado.</param>
    /// <returns>Um <see cref="Result{T}"/> de sucesso contendo <paramref name="value"/>.</returns>
    public static Result<T> ToResult<T>(this T value) => Result<T>.Success(value);
}
