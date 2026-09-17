using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Audit;

/// <summary>Consulta os registros de auditoria de acesso que cruzaram a fronteira de unidades/secretarias (RNF02).</summary>
public sealed record GetCrossUnitAccessLogsQuery : IRequest<Result<IReadOnlyList<CrossAccessResponse>>>;
