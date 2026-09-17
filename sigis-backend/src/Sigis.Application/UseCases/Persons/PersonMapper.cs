using Sigis.Domain.Entities;

namespace Sigis.Application.UseCases.Persons;

/// <summary>Mapeamento compartilhado de <see cref="Person"/> para <see cref="PersonDetailResponse"/>.</summary>
internal static class PersonMapper
{
    /// <summary>Converte uma <see cref="Person"/> completa em seu DTO de resposta, incluindo responsáveis legais.</summary>
    /// <param name="person">Pessoa a ser convertida.</param>
    /// <returns>O <see cref="PersonDetailResponse"/> correspondente.</returns>
    public static PersonDetailResponse ToDetailResponse(Person person) => new(
        person.Id,
        person.Name.Value,
        person.BirthDate,
        person.Cns?.Value,
        person.Cpf?.Value,
        person.MotherName,
        person.Gender,
        person.RaceColor,
        person.Phone?.Value,
        person.Email?.Value,
        person.Address?.Full(),
        person.Naturality,
        person.CurrentSchool,
        person.Grade,
        person.Shift,
        person.ClassGroup,
        person.Zone,
        person.SchoolEnrollment,
        person.ReferredBySchool,
        person.DisabilityTypes,
        person.NeedsSpecialEducation,
        person.AttendsTutoring,
        person.HasFailedGrade,
        person.Guardians.Select(g => new GuardianSummary(g.Id, g.Name.Value, g.Relationship, g.Cns?.Value, g.BirthDate)).ToList(),
        person.CreatedAt,
        person.UpdatedAt);
}
