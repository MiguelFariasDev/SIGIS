using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.ClinicalProfile;

/// <summary>Consulta o perfil clínico NASF de uma pessoa.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
public sealed record GetClinicalProfileQuery(Guid PersonId) : IRequest<Result<PersonClinicalProfileResponse>>;
