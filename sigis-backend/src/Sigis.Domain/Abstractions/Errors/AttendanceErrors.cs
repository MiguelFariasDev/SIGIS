namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros do módulo de Atendimentos e Histórico (<c>Attendance</c>).</summary>
public static class AttendanceErrors
{
    /// <summary>Tipo de sessão é obrigatório.</summary>
    public static readonly Error TipoSessaoObrigatorio = new("ATTENDANCE_001", "Tipo de sessão é obrigatório.");

    /// <summary>Dados do formulário inválidos.</summary>
    public static readonly Error DadosFormularioInvalidos = new("ATTENDANCE_002", "Dados do formulário inválidos.");

    /// <summary>Comparecimento já registrado.</summary>
    public static readonly Error ComparecimentoJaRegistrado = new("ATTENDANCE_003", "Comparecimento já registrado.");

    /// <summary>Não é possível alterar após comparecimento registrado.</summary>
    public static readonly Error NaoPodeAlterarAposComparecimento = new("ATTENDANCE_004", "Não é possível alterar após comparecimento registrado.");
}
