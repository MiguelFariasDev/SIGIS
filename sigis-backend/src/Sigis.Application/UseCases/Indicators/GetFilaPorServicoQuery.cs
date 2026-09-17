using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Indicators;

/// <summary>Consulta a fila (aguardando/em atendimento) agrupada por unidade de serviço, para o painel de indicadores.</summary>
public sealed record GetFilaPorServicoQuery : IRequest<Result<IReadOnlyList<FilaPorServicoResponse>>>;
