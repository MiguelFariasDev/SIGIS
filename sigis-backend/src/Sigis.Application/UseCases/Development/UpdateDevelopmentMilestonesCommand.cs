using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Development;

/// <summary>
/// Comando para criar (se ainda não existir) ou atualizar os marcos de
/// desenvolvimento de uma pessoa (relação 1:1).
/// </summary>
public sealed record UpdateDevelopmentMilestonesCommand(
    Guid PersonId,
    int? AgeWalkedMonths,
    int? AgeTalkedMonths,
    bool? LocomotionDifficulty,
    bool? CoordinationDifficulty,
    bool? VisualDifficulty,
    bool? HearingDifficulty,
    string? SpeechProblems,
    string? CommandComprehension,
    string? CommunicationForm,
    string? ManualDominance) : IRequest<Result<DevelopmentMilestonesResponse>>;
