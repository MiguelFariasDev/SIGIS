namespace Sigis.Domain.Enums;

/// <summary>Situação de comparecimento de um atendimento agendado ou realizado.</summary>
public enum AttendanceStatus
{
    /// <summary>Atendimento agendado, ainda sem comparecimento registrado.</summary>
    Scheduled = 1,

    /// <summary>Pessoa compareceu ao atendimento.</summary>
    Attended = 2,

    /// <summary>Pessoa faltou ao atendimento.</summary>
    Absent = 3
}
