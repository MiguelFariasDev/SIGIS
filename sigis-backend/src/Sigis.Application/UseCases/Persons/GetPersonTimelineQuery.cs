using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Persons;

/// <summary>
/// Consulta a linha do tempo consolidada de uma pessoa (RF08). O
/// profissional autenticado é obtido de <c>ICurrentUserService</c> pelo
/// handler, não passado explicitamente neste comando.
/// </summary>
/// <param name="PersonId">Identificador da pessoa.</param>
/// <param name="Nivel">
/// Nível de disclosure solicitado: "metadados" (datas/tipos/unidades apenas)
/// ou "completo" (inclui motivo/queixa). Qualquer valor diferente de
/// "metadados" é tratado como "completo".
/// </param>
/// <param name="Justificativa">
/// Justificativa do acesso — obrigatória quando o nível solicitado é
/// "completo", ou quando o acesso cruza a fronteira de secretarias (LGPD,
/// art. 11, II), independentemente do nível.
/// </param>
public sealed record GetPersonTimelineQuery(Guid PersonId, string? Nivel, string? Justificativa)
    : IRequest<Result<TimelineResponse>>;
