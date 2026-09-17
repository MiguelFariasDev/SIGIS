namespace Sigis.Api.Contracts.Persons;

/// <summary>Requisição de mesclagem de dois cadastros de pessoa duplicados.</summary>
/// <param name="SourcePersonId">Identificador do cadastro de origem, que será mesclado no destino.</param>
/// <param name="TargetPersonId">Identificador do cadastro de destino (canônico), que permanece ativo.</param>
/// <param name="DuplicateAlertId">
/// Identificador do alerta de duplicidade que motivou a mesclagem, opcional —
/// quando informado, o alerta é marcado como resolvido (mesclado).
/// </param>
public sealed record MergePersonsRequest(Guid SourcePersonId, Guid TargetPersonId, Guid? DuplicateAlertId);
