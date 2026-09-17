using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Learning;

/// <summary>Processa a consulta das dificuldades de aprendizagem registradas para uma pessoa.</summary>
public sealed class GetLearningDifficultiesQueryHandler
    : IRequestHandler<GetLearningDifficultiesQuery, Result<IReadOnlyList<LearningDifficultyResponse>>>
{
    private readonly IPersonRepository _personRepository;
    private readonly ILearningDifficultiesRepository _learningDifficultiesRepository;

    /// <summary>Cria o handler de consulta de dificuldades de aprendizagem.</summary>
    public GetLearningDifficultiesQueryHandler(
        IPersonRepository personRepository, ILearningDifficultiesRepository learningDifficultiesRepository)
    {
        _personRepository = personRepository;
        _learningDifficultiesRepository = learningDifficultiesRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<LearningDifficultyResponse>>> Handle(
        GetLearningDifficultiesQuery request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<IReadOnlyList<LearningDifficultyResponse>>.Failure(PersonErrors.PessoaNaoEncontrada);

        var records = await _learningDifficultiesRepository.GetByPersonAsync(request.PersonId, cancellationToken);

        var response = records
            .Select(r => new LearningDifficultyResponse(r.Id, r.PersonId, r.Type, r.Severity, r.AssessmentDate, r.Notes, r.CreatedAt))
            .ToList();

        return Result<IReadOnlyList<LearningDifficultyResponse>>.Success(response);
    }
}
