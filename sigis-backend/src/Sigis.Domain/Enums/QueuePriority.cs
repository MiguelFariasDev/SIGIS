namespace Sigis.Domain.Enums;

/// <summary>Nível de prioridade de uma pessoa na fila de atendimento de uma unidade.</summary>
public enum QueuePriority
{
    /// <summary>Atendimento urgente — maior prioridade na fila.</summary>
    Urgent = 1,

    /// <summary>Atendimento de curto prazo.</summary>
    ShortTerm = 2,

    /// <summary>Lista de espera — menor prioridade na fila.</summary>
    WaitingList = 3
}
