namespace Sigis.Domain.Enums;

/// <summary>Situação de um alerta de possível duplicidade de cadastro entre duas pessoas.</summary>
public enum DuplicateAlertStatus
{
    /// <summary>Alerta aguardando revisão de um coordenador.</summary>
    Pending = 1,

    /// <summary>Cadastros confirmados como duplicados e mesclados.</summary>
    Merged = 2,

    /// <summary>Alerta revisado e classificado como falso positivo — cadastros são de pessoas diferentes.</summary>
    FalsePositive = 3
}
