namespace Sigis.Domain.Exceptions;

/// <summary>
/// Exceção lançada para representar falhas de infraestrutura (banco de
/// dados, rede, serviços externos) capturadas ou relançadas a partir de
/// código de domínio — nunca usada para regras de negócio ou validação, que
/// devem usar <c>Result</c>/<c>Result&lt;T&gt;</c>.
/// </summary>
public sealed class InfrastructureException : Exception
{
    /// <summary>Cria uma nova <see cref="InfrastructureException"/> com a mensagem informada.</summary>
    /// <param name="message">Mensagem descritiva, em português, da falha de infraestrutura.</param>
    public InfrastructureException(string message) : base(message)
    {
    }

    /// <summary>Cria uma nova <see cref="InfrastructureException"/> com a mensagem e a exceção interna informadas.</summary>
    /// <param name="message">Mensagem descritiva, em português, da falha de infraestrutura.</param>
    /// <param name="innerException">Exceção original de infraestrutura.</param>
    public InfrastructureException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
