using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;
using Sigis.Domain.ValueObjects;

namespace Sigis.Application.UseCases.Auth;

/// <summary>
/// Processa o login de um profissional: valida as credenciais, verifica se
/// o profissional está ativo, gera o token de acesso e registra o instante
/// do login.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de login.</summary>
    public LoginCommandHandler(
        IUserRepository userRepository,
        IServiceUnitRepository serviceUnitRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _serviceUnitRepository = serviceUnitRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var validationResult = new LoginCommandValidator().Validate(request);
        if (!validationResult.IsValid)
            return Result<LoginResponse>.Failure(new Error("AUTH_VALIDATION", validationResult.Errors[0].ErrorMessage));

        var emailResult = EmailAddress.Create(request.Email);
        if (emailResult.IsFailure)
            return Result<LoginResponse>.Failure(AuthErrors.InvalidCredentials);

        var professional = await _userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);

        // Mesma mensagem para e-mail inexistente e senha incorreta — nunca
        // revelar se um e-mail está cadastrado (regra de ouro de auth).
        if (professional is null)
            return Result<LoginResponse>.Failure(AuthErrors.InvalidCredentials);

        if (!professional.Active)
            return Result<LoginResponse>.Failure(AuthErrors.UserInactive);

        if (!_passwordHasher.Verify(request.Password, professional.PasswordHash))
            return Result<LoginResponse>.Failure(AuthErrors.InvalidCredentials);

        professional.RegisterLogin(_dateTimeProvider.UtcNow);
        await _userRepository.UpdateAsync(professional, cancellationToken);

        var tokenResult = _tokenService.GenerateToken(professional);
        if (tokenResult.IsFailure)
            return Result<LoginResponse>.Failure(tokenResult.Error);

        var unit = await _serviceUnitRepository.GetByIdAsync(professional.UnitId, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userSummary = new LoginUserSummary(
            professional.Id,
            professional.Name.Value,
            professional.Email.Value,
            professional.Role.ToString(),
            professional.UnitId,
            unit?.Name ?? string.Empty,
            unit?.Acronym ?? string.Empty,
            unit?.Secretariat.ToString() ?? string.Empty);

        return Result<LoginResponse>.Success(
            new LoginResponse(tokenResult.Value.AccessToken, tokenResult.Value.ExpiresInSeconds, userSummary));
    }
}
