namespace Sigis.Domain.Enums;

/// <summary>Situação de um encaminhamento de pessoa entre unidades da rede.</summary>
public enum ReferralStatus
{
    /// <summary>Encaminhamento aguardando avaliação da unidade de destino.</summary>
    Pending = 1,

    /// <summary>Encaminhamento aceito pela unidade de destino.</summary>
    Accepted = 2,

    /// <summary>Encaminhamento concluído após o primeiro atendimento na unidade de destino.</summary>
    Completed = 3,

    /// <summary>Encaminhamento recusado pela unidade de destino.</summary>
    Refused = 4
}
