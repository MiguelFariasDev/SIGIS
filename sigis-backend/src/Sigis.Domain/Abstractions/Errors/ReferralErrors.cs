namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros do módulo de Encaminhamentos (<c>Referral</c>).</summary>
public static class ReferralErrors
{
    /// <summary>Unidade de origem e destino não podem ser iguais.</summary>
    public static readonly Error OrigemIgualDestino = new("REFERRAL_001", "Unidade de origem e destino não podem ser iguais.");

    /// <summary>Motivo do encaminhamento é obrigatório.</summary>
    public static readonly Error MotivoObrigatorio = new("REFERRAL_002", "Motivo do encaminhamento é obrigatório.");

    /// <summary>Motivo deve ter ao menos 10 caracteres.</summary>
    public static readonly Error MotivoMuitoCurto = new("REFERRAL_003", "Motivo deve ter ao menos 10 caracteres.");

    /// <summary>Transição de status inválida para encaminhamento.</summary>
    public static readonly Error TransicaoInvalida = new("REFERRAL_004", "Transição de status inválida para encaminhamento.");

    /// <summary>Encaminhamento precisa ser aceito antes de concluir.</summary>
    public static readonly Error NaoAceito = new("REFERRAL_005", "Encaminhamento precisa ser aceito antes de concluir.");
}
