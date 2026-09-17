using FluentValidation;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;

namespace Sigis.Domain.ValueObjects;

/// <summary>Endereço de e-mail de contato de uma pessoa.</summary>
public sealed record EmailAddress
{
    /// <summary>Endereço de e-mail, normalizado em minúsculas.</summary>
    public string Value { get; }

    private EmailAddress(string value) => Value = value;

    /// <summary>
    /// Cria um <see cref="EmailAddress"/> válido a partir de uma string bruta.
    /// </summary>
    /// <param name="rawEmail">Endereço de e-mail informado.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso com o e-mail normalizado em
    /// minúsculas, ou de falha com <see cref="PersonErrors.EmailInvalido"/>.
    /// </returns>
    public static Result<EmailAddress> Create(string? rawEmail)
    {
        var trimmed = (rawEmail ?? string.Empty).Trim();

        var validationResult = new Validator().Validate(trimmed);
        if (!validationResult.IsValid)
            return Result<EmailAddress>.Failure(PersonErrors.EmailInvalido);

        return Result<EmailAddress>.Success(new EmailAddress(trimmed.ToLowerInvariant()));
    }

    /// <summary>
    /// Validador interno (FluentValidation) do e-mail: obrigatório, no
    /// máximo 254 caracteres (limite prático do padrão RFC 5321) e com
    /// formato de e-mail válido.
    /// </summary>
    private sealed class Validator : AbstractValidator<string>
    {
        public Validator()
        {
            RuleFor(x => x)
                .NotEmpty()
                .MaximumLength(254)
                .EmailAddress();
        }
    }
}
