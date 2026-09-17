using FluentValidation;

namespace Sigis.Application.UseCases.Auth;

/// <summary>Validador de <see cref="LoginCommand"/>: e-mail e senha são obrigatórios.</summary>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    /// <summary>Cria o validador de <see cref="LoginCommand"/>.</summary>
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(8).WithMessage("Senha deve ter ao menos 8 caracteres.");
    }
}
