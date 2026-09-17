namespace Sigis.Domain.Exceptions;

/// <summary>
/// Exceção lançada quando uma invariante do domínio é violada de uma forma
/// que não pode ser expressa como um <see cref="Sigis.Domain.Abstractions.Result"/>
/// de falha — reservada a bugs de programação (ex.: uso incorreto de uma API
/// interna do domínio por outra camada). Nunca deve ser usada para fluxo
/// esperado de validação ou regra de negócio, que deve retornar
/// <c>Result</c>/<c>Result&lt;T&gt;</c>.
/// </summary>
public sealed class DomainException : Exception
{
    /// <summary>Cria uma nova <see cref="DomainException"/> com a mensagem informada.</summary>
    /// <param name="message">Mensagem descritiva, em português, da invariante violada.</param>
    public DomainException(string message) : base(message)
    {
    }

    /// <summary>Cria uma nova <see cref="DomainException"/> com a mensagem e a exceção interna informadas.</summary>
    /// <param name="message">Mensagem descritiva, em português, da invariante violada.</param>
    /// <param name="innerException">Exceção que causou esta invariante.</param>
    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
