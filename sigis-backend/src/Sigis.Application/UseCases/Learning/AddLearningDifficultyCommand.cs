using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Enums;

namespace Sigis.Application.UseCases.Learning;

/// <summary>Comando para registrar uma dificuldade de aprendizagem de uma pessoa.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
/// <param name="Type">Tipo de dificuldade.</param>
/// <param name="Severity">Gravidade, opcional.</param>
/// <param name="AssessmentDate">Data da sondagem psicopedagógica, opcional.</param>
/// <param name="Notes">Observações, opcionais.</param>
public sealed record AddLearningDifficultyCommand(
    Guid PersonId,
    LearningDifficultyType Type,
    string? Severity,
    DateOnly? AssessmentDate,
    string? Notes) : IRequest<Result<LearningDifficultyResponse>>;
