using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.ConcurrentTreatments;

/// <summary>Consulta os atendimentos concomitantes de uma pessoa.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
public sealed record GetConcurrentTreatmentsQuery(Guid PersonId) : IRequest<Result<IReadOnlyList<ConcurrentTreatmentResponse>>>;
