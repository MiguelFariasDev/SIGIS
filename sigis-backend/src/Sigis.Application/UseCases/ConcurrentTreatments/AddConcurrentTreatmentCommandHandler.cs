using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.ConcurrentTreatments;

/// <summary>
/// Processa o registro de um atendimento concomitante de uma pessoa,
/// aplicando RN-CT02 (sem sobreposição de horário no mesmo dia da semana)
/// antes de delegar as demais validações à entidade.
/// </summary>
public sealed class AddConcurrentTreatmentCommandHandler
    : IRequestHandler<AddConcurrentTreatmentCommand, Result<ConcurrentTreatmentResponse>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IConcurrentTreatmentRepository _concurrentTreatmentRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de registro de atendimento concomitante.</summary>
    public AddConcurrentTreatmentCommandHandler(
        IPersonRepository personRepository,
        IConcurrentTreatmentRepository concurrentTreatmentRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _personRepository = personRepository;
        _concurrentTreatmentRepository = concurrentTreatmentRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<ConcurrentTreatmentResponse>> Handle(
        AddConcurrentTreatmentCommand request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<ConcurrentTreatmentResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        var existingTreatments = await _concurrentTreatmentRepository.GetByPersonAsync(request.PersonId, cancellationToken);
        if (existingTreatments.Any(t => t.OverlapsWith(request.DayOfWeek, request.StartTime, request.EndTime)))
            return Result<ConcurrentTreatmentResponse>.Failure(ConcurrentTreatmentErrors.SobreposicaoDeHorario);

        var createResult = Sigis.Domain.Entities.ConcurrentTreatment.Create(
            request.PersonId, request.Specialty, request.Location, request.ProfessionalName, request.DayOfWeek,
            request.StartTime, request.EndTime, _dateTimeProvider.UtcNow, request.Notes);

        if (createResult.IsFailure)
            return Result<ConcurrentTreatmentResponse>.Failure(createResult.Error);

        var treatment = createResult.Value;
        await _concurrentTreatmentRepository.AddAsync(treatment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ConcurrentTreatmentResponse>.Success(new ConcurrentTreatmentResponse(
            treatment.Id, treatment.PersonId, treatment.Specialty, treatment.Location, treatment.ProfessionalName,
            treatment.DayOfWeek, treatment.StartTime, treatment.EndTime, treatment.Notes, treatment.CreatedAt, treatment.UpdatedAt));
    }
}
