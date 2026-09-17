using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Duplicates;

/// <summary>
/// Resolve um alerta de duplicidade pendente, delegando para a mesclagem
/// (<see cref="Sigis.Application.UseCases.Persons.MergePersonsCommand"/>) ou
/// para a marcação como falso positivo, conforme a ação escolhida.
/// </summary>
/// <param name="AlertId">Identificador do alerta de duplicidade.</param>
/// <param name="Acao">Ação escolhida: <c>"MESCLAR"</c> ou <c>"FALSO_POSITIVO"</c>.</param>
public sealed record ResolveDuplicateCommand(Guid AlertId, string Acao) : IRequest<Result<DuplicateAlertResponse>>;
