namespace Sigis.Domain.Enums;

/// <summary>Situação atual de uma entrada na fila de atendimento de uma unidade.</summary>
public enum QueueStatus
{
    /// <summary>Pessoa aguardando ser chamada para atendimento.</summary>
    Waiting = 1,

    /// <summary>Pessoa em atendimento no momento.</summary>
    InAttendance = 2,

    /// <summary>Atendimento concluído — a pessoa saiu da fila.</summary>
    Completed = 3,

    /// <summary>Pessoa faltou ao atendimento agendado.</summary>
    Absent = 4,

    /// <summary>Pessoa está em processo de busca ativa após faltas consecutivas.</summary>
    ActiveSearch = 5
}
