using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Duplicates;

/// <summary>Consulta os alertas de duplicidade pendentes de revisão (RF04), paginados.</summary>
/// <param name="Skip">Quantidade de registros a pular.</param>
/// <param name="Take">Quantidade máxima de registros retornados.</param>
public sealed record GetPendingDuplicatesQuery(int Skip, int Take) : IRequest<Result<IReadOnlyList<DuplicateAlertResponse>>>;
