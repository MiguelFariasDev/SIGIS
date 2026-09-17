using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.SchoolHistories;

/// <summary>
/// Processa o registro de um novo vínculo escolar de uma pessoa, aplicando
/// RN-SH01 (apenas um registro <c>Ativo</c> por pessoa) antes de delegar as
/// demais validações à entidade.
/// </summary>
public sealed class AddSchoolHistoryCommandHandler : IRequestHandler<AddSchoolHistoryCommand, Result<SchoolHistoryResponse>>
{
    private readonly IPersonRepository _personRepository;
    private readonly ISchoolHistoryRepository _schoolHistoryRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de registro de histórico escolar.</summary>
    public AddSchoolHistoryCommandHandler(
        IPersonRepository personRepository,
        ISchoolHistoryRepository schoolHistoryRepository,
        ICurrentUserService currentUserService,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _schoolHistoryRepository = schoolHistoryRepository;
        _currentUserService = currentUserService;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<SchoolHistoryResponse>> Handle(AddSchoolHistoryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = new AddSchoolHistoryCommandValidator().Validate(request);
        if (!validationResult.IsValid)
            return Result<SchoolHistoryResponse>.Failure(new Error("SCHOOL_VALIDATION", validationResult.Errors[0].ErrorMessage));

        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<SchoolHistoryResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        var activeRecord = await _schoolHistoryRepository.GetActiveByPersonAsync(request.PersonId, cancellationToken);
        if (activeRecord is not null)
            return Result<SchoolHistoryResponse>.Failure(SchoolHistoryErrors.EscolaAtivaJaExiste);

        var professionalId = _currentUserService.ProfessionalId!.Value;
        var now = _dateTimeProvider.UtcNow;

        var createResult = Sigis.Domain.Entities.SchoolHistory.Create(
            request.PersonId, request.SchoolName, request.Grade, request.SchoolYear, professionalId, now,
            _dateTimeProvider.Today.Year, request.Shift, request.ClassGroup, request.StartDate, request.Notes);

        if (createResult.IsFailure)
            return Result<SchoolHistoryResponse>.Failure(createResult.Error);

        var record = createResult.Value;
        await _schoolHistoryRepository.AddAsync(record, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<SchoolHistoryResponse>.Success(new SchoolHistoryResponse(
            record.Id, record.PersonId, record.SchoolName, record.Grade, record.Shift, record.ClassGroup,
            record.SchoolYear, record.StartDate, record.EndDate, record.Status, record.Notes,
            record.CreatedAt, record.CreatedByProfessionalId));
    }
}
