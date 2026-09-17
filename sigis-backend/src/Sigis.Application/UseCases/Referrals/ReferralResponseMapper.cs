using Sigis.Domain.Entities;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Referrals;

/// <summary>
/// Monta o <see cref="ReferralResponse"/> de um <see cref="Referral"/>,
/// resolvendo os nomes das unidades de origem e destino — usado pelos
/// handlers deste módulo para evitar repetir a mesma lógica de projeção.
/// </summary>
internal static class ReferralResponseMapper
{
    /// <summary>Converte um <see cref="Referral"/> em um <see cref="ReferralResponse"/>.</summary>
    /// <param name="referral">Encaminhamento a converter.</param>
    /// <param name="serviceUnitRepository">Repositório de unidades de serviço, para resolver os nomes.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O <see cref="ReferralResponse"/> correspondente.</returns>
    public static async Task<ReferralResponse> MapAsync(
        Referral referral, IServiceUnitRepository serviceUnitRepository, CancellationToken cancellationToken)
    {
        var originUnit = await serviceUnitRepository.GetByIdAsync(referral.OriginUnitId, cancellationToken);
        var destinationUnit = await serviceUnitRepository.GetByIdAsync(referral.DestinationUnitId, cancellationToken);

        return new ReferralResponse(
            referral.Id,
            referral.PersonId,
            referral.OriginUnitId,
            originUnit?.Name ?? string.Empty,
            referral.DestinationUnitId,
            destinationUnit?.Name ?? string.Empty,
            referral.Reason,
            referral.Priority.ToString(),
            referral.ReferralDate,
            referral.Status.ToString(),
            referral.CorrelationId);
    }
}
