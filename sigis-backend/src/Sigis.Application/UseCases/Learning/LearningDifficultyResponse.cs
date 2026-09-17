using Sigis.Domain.Enums;

namespace Sigis.Application.UseCases.Learning;

/// <summary>Representação de uma dificuldade de aprendizagem retornada pela API.</summary>
public sealed record LearningDifficultyResponse(
    Guid Id,
    Guid PersonId,
    LearningDifficultyType Type,
    string? Severity,
    DateOnly? AssessmentDate,
    string? Notes,
    DateTime CreatedAt);
