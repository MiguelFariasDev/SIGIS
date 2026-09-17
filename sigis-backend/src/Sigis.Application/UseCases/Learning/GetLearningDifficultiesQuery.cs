using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Learning;

/// <summary>Consulta as dificuldades de aprendizagem registradas para uma pessoa.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
public sealed record GetLearningDifficultiesQuery(Guid PersonId) : IRequest<Result<IReadOnlyList<LearningDifficultyResponse>>>;
