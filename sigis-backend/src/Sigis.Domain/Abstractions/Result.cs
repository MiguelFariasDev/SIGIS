namespace Sigis.Domain.Abstractions;

/// <summary>
/// Representa o resultado de uma operação que pode falhar por uma regra de
/// negócio, evitando o uso de exceções para fluxo de controle esperado.
/// Métodos de domínio que podem falhar retornam <see cref="Result"/> ou
/// <see cref="Result{T}"/> em vez de lançar exceções.
/// </summary>
public class Result
{
    /// <summary>Indica se a operação foi concluída com sucesso.</summary>
    public bool IsSuccess { get; }

    /// <summary>Indica se a operação falhou.</summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>Erro associado à falha. É <see cref="Error.None"/> quando <see cref="IsSuccess"/> é verdadeiro.</summary>
    public Error Error { get; }

    /// <summary>
    /// Inicializa uma nova instância de <see cref="Result"/>.
    /// </summary>
    /// <param name="isSuccess">Indica se a operação foi bem-sucedida.</param>
    /// <param name="error">Erro associado, ou <see cref="Error.None"/> em caso de sucesso.</param>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando um resultado de sucesso é criado com um erro, ou quando
    /// um resultado de falha é criado sem erro — ambos os casos indicam bug de
    /// programação no código que constrói o <see cref="Result"/>.
    /// </exception>
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Result de sucesso não pode ter erro.");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Result de falha deve ter erro.");

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>Cria um resultado de sucesso sem valor associado.</summary>
    /// <returns>Um <see cref="Result"/> bem-sucedido.</returns>
    public static Result Success() => new(true, Error.None);

    /// <summary>Cria um resultado de falha com o erro informado.</summary>
    /// <param name="error">Erro que causou a falha.</param>
    /// <returns>Um <see cref="Result"/> de falha.</returns>
    public static Result Failure(Error error) => new(false, error);

    /// <summary>Cria um resultado de sucesso contendo um valor.</summary>
    /// <typeparam name="T">Tipo do valor retornado.</typeparam>
    /// <param name="value">Valor produzido pela operação.</param>
    /// <returns>Um <see cref="Result{T}"/> bem-sucedido.</returns>
    public static Result<T> Success<T>(T value) => Result<T>.Success(value);

    /// <summary>Cria um resultado de falha tipado com o erro informado.</summary>
    /// <typeparam name="T">Tipo do valor que seria retornado em caso de sucesso.</typeparam>
    /// <param name="error">Erro que causou a falha.</param>
    /// <returns>Um <see cref="Result{T}"/> de falha.</returns>
    public static Result<T> Failure<T>(Error error) => Result<T>.Failure(error);

    /// <summary>
    /// Executa uma das duas funções conforme o resultado seja sucesso ou falha.
    /// </summary>
    /// <typeparam name="TResult">Tipo do valor retornado por ambas as funções.</typeparam>
    /// <param name="onSuccess">Função executada quando o resultado é sucesso.</param>
    /// <param name="onFailure">Função executada quando o resultado é falha, recebendo o erro.</param>
    /// <returns>O valor produzido pela função correspondente ao estado do resultado.</returns>
    public TResult Match<TResult>(Func<TResult> onSuccess, Func<Error, TResult> onFailure)
        => IsSuccess ? onSuccess() : onFailure(Error);
}

/// <summary>
/// Representa o resultado de uma operação que pode falhar por uma regra de
/// negócio e que, em caso de sucesso, produz um valor do tipo <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">Tipo do valor produzido em caso de sucesso.</typeparam>
public class Result<T> : Result
{
    private readonly T? _value;

    /// <summary>
    /// Valor produzido pela operação, disponível apenas quando
    /// <see cref="Result.IsSuccess"/> é verdadeiro.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Lançada ao tentar acessar o valor de um resultado de falha — indica bug
    /// de programação no código que consome o <see cref="Result{T}"/> sem
    /// verificar <see cref="Result.IsSuccess"/> antes.
    /// </exception>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Não é possível acessar Value de um Result de falha.");

    private Result(bool isSuccess, T? value, Error error) : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>Cria um resultado de sucesso contendo o valor informado.</summary>
    /// <param name="value">Valor produzido pela operação.</param>
    /// <returns>Um <see cref="Result{T}"/> bem-sucedido.</returns>
    public static Result<T> Success(T value) => new(true, value, Error.None);

    /// <summary>Cria um resultado de falha tipado com o erro informado.</summary>
    /// <param name="error">Erro que causou a falha.</param>
    /// <returns>Um <see cref="Result{T}"/> de falha.</returns>
    public static new Result<T> Failure(Error error) => new(false, default, error);

    /// <summary>Converte implicitamente um valor em um <see cref="Result{T}"/> de sucesso.</summary>
    /// <param name="value">Valor a ser encapsulado em um resultado de sucesso.</param>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Executa uma das duas funções conforme o resultado seja sucesso ou falha.
    /// </summary>
    /// <typeparam name="TResult">Tipo do valor retornado por ambas as funções.</typeparam>
    /// <param name="onSuccess">Função executada quando o resultado é sucesso, recebendo o valor produzido.</param>
    /// <param name="onFailure">Função executada quando o resultado é falha, recebendo o erro.</param>
    /// <returns>O valor produzido pela função correspondente ao estado do resultado.</returns>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<Error, TResult> onFailure)
        => IsSuccess ? onSuccess(_value!) : onFailure(Error);
}
