namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros do módulo de Fila de Atendimento (<c>QueueEntry</c>).</summary>
public static class QueueErrors
{
    /// <summary>Já existe fila ativa para esta pessoa nesta unidade.</summary>
    public static readonly Error FilaJaAtiva = new("QUEUE_001", "Já existe fila ativa para esta pessoa nesta unidade.");

    /// <summary>Fila não está ativa.</summary>
    public static readonly Error FilaNaoAtiva = new("QUEUE_002", "Fila não está ativa.");

    /// <summary>Transição de status inválida para fila.</summary>
    public static readonly Error TransicaoInvalida = new("QUEUE_003", "Transição de status inválida para fila.");

    /// <summary>Justificativa é obrigatória para reclassificação.</summary>
    public static readonly Error JustificativaObrigatoria = new("QUEUE_004", "Justificativa é obrigatória para reclassificação.");

    /// <summary>Justificativa deve ter ao menos 10 caracteres.</summary>
    public static readonly Error JustificativaMuitoCurta = new("QUEUE_005", "Justificativa deve ter ao menos 10 caracteres.");

    /// <summary>Especialidade é obrigatória.</summary>
    public static readonly Error EspecialidadeObrigatoria = new("QUEUE_006", "Especialidade é obrigatória.");
}
