namespace Sigis.Application.UseCases.Persons;

/// <summary>Resultado da mesclagem de dois cadastros de pessoa duplicados.</summary>
/// <param name="TargetPersonId">Identificador do cadastro canônico, para o qual os registros foram reatribuídos.</param>
/// <param name="SourcePersonId">Identificador do cadastro de origem, mesclado no destino.</param>
/// <param name="AttendancesReassigned">Quantidade de atendimentos reatribuídos.</param>
/// <param name="ReferralsReassigned">Quantidade de encaminhamentos reatribuídos.</param>
/// <param name="QueueEntriesReassigned">Quantidade de entradas de fila reatribuídas.</param>
/// <param name="GuardiansCopied">Quantidade de responsáveis legais copiados para o cadastro de destino.</param>
public sealed record MergeResult(
    Guid TargetPersonId,
    Guid SourcePersonId,
    int AttendancesReassigned,
    int ReferralsReassigned,
    int QueueEntriesReassigned,
    int GuardiansCopied);
