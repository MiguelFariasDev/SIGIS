namespace Sigis.Domain.Interfaces;

/// <summary>
/// Abstrai o acesso à data e hora atuais, permitindo que a camada de
/// aplicação injete um relógio determinístico nos testes em vez de depender
/// de <see cref="DateTime.UtcNow"/> diretamente.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>Data e hora atuais, em UTC.</summary>
    DateTime UtcNow { get; }

    /// <summary>Data atual (sem componente de hora), usada para validações como data de nascimento.</summary>
    DateOnly Today { get; }
}
