using MediatR;
using Sigis.Domain.Abstractions;

namespace Sigis.Application.UseCases.Persons;

/// <summary>Mescla dois cadastros de pessoa reconhecidos como duplicados, sob decisão de um coordenador (RN02).</summary>
/// <param name="SourcePersonId">Identificador do cadastro de origem, que será mesclado no destino.</param>
/// <param name="TargetPersonId">Identificador do cadastro de destino (canônico), que permanece ativo.</param>
/// <param name="DuplicateAlertId">
/// Identificador do alerta de duplicidade que motivou a mesclagem, opcional
/// — quando informado, o alerta é marcado como resolvido (mesclado).
/// </param>
public sealed record MergePersonsCommand(Guid SourcePersonId, Guid TargetPersonId, Guid? DuplicateAlertId)
    : IRequest<Result<MergeResult>>;
