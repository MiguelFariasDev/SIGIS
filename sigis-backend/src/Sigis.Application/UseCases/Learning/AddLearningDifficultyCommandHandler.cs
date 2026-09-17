using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Learning;

/// <summary>
/// Processa o registro de uma dificuldade de aprendizagem de uma pessoa,
/// aplicando RN-LD01 (um único registro por tipo por pessoa) antes de
/// delegar as demais validações à entidade.
/// </summary>
public sealed class AddLearningDifficultyCommandHandler
    : IRequestHandler<AddLearningDifficultyCommand, Result<LearningDifficultyResponse>>
{
    private readonly IPersonRepository _personRepository;
    private readonly ILearningDifficultiesRepository _learningDifficultiesRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de registro de dificuldade de aprendizagem.</summary>
    public AddLearningDifficultyCommandHandler(
        IPersonRepository personRepository,
        ILearningDifficultiesRepository learningDifficultiesRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _learningDifficultiesRepository = learningDifficultiesRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<LearningDifficultyResponse>> Handle(
        AddLearningDifficultyCommand request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<LearningDifficultyResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        var existing = await _learningDifficultiesRepository.GetByPersonAndTypeAsync(
            request.PersonId, request.Type, cancellationToken);
        if (existing is not null)
            return Result<LearningDifficultyResponse>.Failure(LearningDifficultiesErrors.RegistroDuplicado);

        var createResult = Sigis.Domain.Entities.LearningDifficulties.Create(
            request.PersonId, request.Type, _dateTimeProvider.UtcNow, _dateTimeProvider.Today,
            request.Severity, request.AssessmentDate, request.Notes);

        if (createResult.IsFailure)
            return Result<LearningDifficultyResponse>.Failure(createResult.Error);

        var record = createResult.Value;
        await _learningDifficultiesRepository.AddAsync(record, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<LearningDifficultyResponse>.Success(new LearningDifficultyResponse(
            record.Id, record.PersonId, record.Type, record.Severity, record.AssessmentDate, record.Notes, record.CreatedAt));
    }
}
