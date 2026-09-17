using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Audit;

/// <summary>Resolve os nomes de pessoa e profissional de um <see cref="AccessLog"/> para exibição em auditoria.</summary>
public static class AccessLogResponseMapper
{
    /// <summary>Mapeia um <see cref="AccessLog"/> para <see cref="AccessLogResponse"/>.</summary>
    public static async Task<AccessLogResponse> MapAsync(
        AccessLog log, IPersonRepository personRepository, IProfessionalRepository professionalRepository,
        CancellationToken cancellationToken)
    {
        var person = await personRepository.GetByIdAsync(log.PersonId, cancellationToken);
        var professional = await professionalRepository.GetByIdAsync(log.ProfessionalId, cancellationToken);

        return new AccessLogResponse(
            log.Id,
            log.PersonId,
            person?.Name.Value ?? "Pessoa não encontrada",
            log.ProfessionalId,
            professional?.Name.Value ?? "Profissional não encontrado",
            log.Action,
            log.LegalBasis,
            log.Justification,
            log.DateTime,
            log.IsCrossUnit());
    }

    /// <summary>Mapeia um <see cref="AccessLog"/> para <see cref="CrossAccessResponse"/>.</summary>
    public static async Task<CrossAccessResponse> MapCrossAsync(
        AccessLog log, IPersonRepository personRepository, IProfessionalRepository professionalRepository,
        CancellationToken cancellationToken)
    {
        var person = await personRepository.GetByIdAsync(log.PersonId, cancellationToken);
        var professional = await professionalRepository.GetByIdAsync(log.ProfessionalId, cancellationToken);

        return new CrossAccessResponse(
            log.Id,
            log.PersonId,
            person?.Name.Value ?? "Pessoa não encontrada",
            log.ProfessionalId,
            professional?.Name.Value ?? "Profissional não encontrado",
            log.Action,
            log.LegalBasis,
            log.Justification,
            log.DateTime);
    }
}
