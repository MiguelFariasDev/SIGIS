using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Family;

/// <summary>
/// Comando para criar (se ainda não existir) ou atualizar a composição
/// familiar de uma pessoa. Relação 1:1 — sempre reflete o estado mais
/// recente enviado pelo cliente.
/// </summary>
public sealed record UpdateFamilyCompositionCommand(
    Guid PersonId,
    string? FatherName,
    string? FatherEducation,
    string? FatherOccupation,
    string? MotherName,
    string? MotherEducation,
    string? MotherOccupation,
    int? SiblingsCount,
    string? SiblingsAges,
    int? HouseholdMembersCount,
    string? ParentsMaritalStatus,
    string? FiliationType,
    bool? PlannedPregnancy,
    int? PregnanciesCount,
    int? AbortionsCount,
    string? PregnancyHealthIssue,
    string? DeliveryType,
    string? MedicationDuringPregnancy) : IRequest<Result<FamilyCompositionResponse>>;
