using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Persons;

/// <summary>Processa a solicitação de acesso completo a um cadastro fora da unidade do profissional (LGPD, art. 11, II).</summary>
public sealed class RequestPersonAccessCommandHandler : IRequestHandler<RequestPersonAccessCommand, Result<PersonDetailResponse>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de solicitação de acesso.</summary>
    public RequestPersonAccessCommandHandler(
        IPersonRepository personRepository,
        IAccessLogRepository accessLogRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _accessLogRepository = accessLogRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<PersonDetailResponse>> Handle(RequestPersonAccessCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Justificativa))
            return Result<PersonDetailResponse>.Failure(AccessLogErrors.JustificativaObrigatoriaCrossUnidade);

        if (_currentUserService.ProfessionalId is not Guid professionalId)
            return Result<PersonDetailResponse>.Failure(AuthErrors.Unauthorized);

        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<PersonDetailResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        var accessLogResult = AccessLog.Create(
            request.PersonId,
            professionalId,
            action: "SolicitacaoAcesso",
            // Coluna access_log.legal_basis é varchar(50) — texto precisa caber.
            legalBasis: "LGPD art. 11, II — políticas públicas",
            dateTime: _dateTimeProvider.UtcNow,
            isCrossUnit: true,
            justification: request.Justificativa);

        if (accessLogResult.IsFailure)
            return Result<PersonDetailResponse>.Failure(accessLogResult.Error);

        await _accessLogRepository.AddAsync(accessLogResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PersonDetailResponse>.Success(PersonMapper.ToDetailResponse(person));
    }
}
