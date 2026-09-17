namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros do módulo de Atendimentos Concomitantes (<c>ConcurrentTreatment</c>).</summary>
public static class ConcurrentTreatmentErrors
{
    /// <summary>Horário de início deve ser anterior ao horário de fim.</summary>
    public static readonly Error HorarioInicioAposFim = new("CONCURRENT_001", "Horário de início deve ser anterior ao horário de fim.");

    /// <summary>Já existe tratamento neste horário.</summary>
    public static readonly Error SobreposicaoDeHorario = new("CONCURRENT_002", "Já existe tratamento neste horário.", ErrorType.Conflict);

    /// <summary>Especialidade é obrigatória.</summary>
    public static readonly Error EspecialidadeObrigatoria = new("CONCURRENT_003", "Especialidade é obrigatória.");

    /// <summary>Local é obrigatório.</summary>
    public static readonly Error LocalObrigatorio = new("CONCURRENT_004", "Local é obrigatório.");
}
