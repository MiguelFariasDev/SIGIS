using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.ConcurrentTreatments;

/// <summary>Comando para remover um atendimento concomitante de uma pessoa.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
/// <param name="TreatmentId">Identificador do atendimento concomitante a remover.</param>
public sealed record RemoveConcurrentTreatmentCommand(Guid PersonId, Guid TreatmentId) : IRequest<Result>;
