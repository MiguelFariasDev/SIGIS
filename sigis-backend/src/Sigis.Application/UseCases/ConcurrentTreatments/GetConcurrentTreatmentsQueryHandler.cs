using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.ConcurrentTreatments;

/// <summary>Processa a consulta dos atendimentos concomitantes de uma pessoa.</summary>
public sealed class GetConcurrentTreatmentsQueryHandler
    : IRequestHandler<GetConcurrentTreatmentsQuery, Result<IReadOnlyList<ConcurrentTreatmentResponse>>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IConcurrentTreatmentRepository _concurrentTreatmentRepository;

    /// <summary>Cria o handler de consulta de atendimentos concomitantes.</summary>
    public GetConcurrentTreatmentsQueryHandler(
        IPersonRepository personRepository, IConcurrentTreatmentRepository concurrentTreatmentRepository)
    {
        _personRepository = personRepository;
        _concurrentTreatmentRepository = concurrentTreatmentRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<ConcurrentTreatmentResponse>>> Handle(
        GetConcurrentTreatmentsQuery request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<IReadOnlyList<ConcurrentTreatmentResponse>>.Failure(PersonErrors.PessoaNaoEncontrada);

        var treatments = await _concurrentTreatmentRepository.GetByPersonAsync(request.PersonId, cancellationToken);

        var response = treatments
            .Select(t => new ConcurrentTreatmentResponse(
                t.Id, t.PersonId, t.Specialty, t.Location, t.ProfessionalName, t.DayOfWeek, t.StartTime, t.EndTime,
                t.Notes, t.CreatedAt, t.UpdatedAt))
            .ToList();

        return Result<IReadOnlyList<ConcurrentTreatmentResponse>>.Success(response);
    }
}
