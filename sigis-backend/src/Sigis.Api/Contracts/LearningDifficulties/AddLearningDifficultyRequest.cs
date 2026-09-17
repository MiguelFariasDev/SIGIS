using Sigis.Domain.Enums;

namespace Sigis.Api.Contracts.LearningDifficulties;

/// <summary>Requisição para registrar uma dificuldade de aprendizagem de uma pessoa.</summary>
/// <param name="Type">Tipo de dificuldade.</param>
/// <param name="Severity">Gravidade ("Leve", "Moderada" ou "Severa"), opcional.</param>
/// <param name="AssessmentDate">Data da sondagem psicopedagógica, opcional.</param>
/// <param name="Notes">Observações, opcionais.</param>
public sealed record AddLearningDifficultyRequest(
    LearningDifficultyType Type,
    string? Severity,
    DateOnly? AssessmentDate,
    string? Notes);
