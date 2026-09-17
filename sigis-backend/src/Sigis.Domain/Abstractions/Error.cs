namespace Sigis.Domain.Abstractions;

/// <summary>
/// Representa um erro de domínio, identificado por um código estável e uma
/// mensagem legível em português, usado por <see cref="Result"/> e
/// <see cref="Result{T}"/> para comunicar falhas de regra de negócio sem
/// lançar exceções.
/// </summary>
/// <param name="Code">Código estável do erro, no padrão "MODULO_NUMERO" (ex.: "PERSON_001").</param>
/// <param name="Message">Mensagem descritiva do erro, em português, adequada para exibição ao usuário.</param>
public sealed record Error(string Code, string Message)
{
    /// <summary>Representa a ausência de erro, usada internamente por um <see cref="Result"/> de sucesso.</summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>Indica se este erro representa a ausência de erro (<see cref="None"/>).</summary>
    public bool IsNone => string.IsNullOrEmpty(Code);
}
