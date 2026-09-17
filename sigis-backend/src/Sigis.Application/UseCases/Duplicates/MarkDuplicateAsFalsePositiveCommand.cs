using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Duplicates;

/// <summary>Marca um alerta de duplicidade como falso positivo — os dois cadastros são de pessoas diferentes.</summary>
/// <param name="DuplicateAlertId">Identificador do alerta de duplicidade.</param>
public sealed record MarkDuplicateAsFalsePositiveCommand(Guid DuplicateAlertId) : IRequest<Result>;
