using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.SchoolHistories;

/// <summary>Consulta o histórico escolar completo de uma pessoa.</summary>
/// <param name="PersonId">Identificador da pessoa.</param>
public sealed record GetSchoolHistoryQuery(Guid PersonId) : IRequest<Result<IReadOnlyList<SchoolHistoryResponse>>>;
