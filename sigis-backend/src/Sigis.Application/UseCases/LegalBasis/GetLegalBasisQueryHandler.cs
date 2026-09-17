using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Enums;

namespace Sigis.Application.UseCases.LegalBasis;

/// <summary>
/// Processa <see cref="GetLegalBasisQuery"/>, retornando a tabela estática de
/// bases legais (LGPD, art. 7º e 11) por secretaria responsável — não há
/// entidade nem repositório para este módulo, pois o conteúdo é referência
/// jurídica fixa, não dado de negócio mutável.
/// </summary>
public sealed class GetLegalBasisQueryHandler
    : IRequestHandler<GetLegalBasisQuery, Result<IReadOnlyList<LegalBasisResponse>>>
{
    private static readonly IReadOnlyList<LegalBasisResponse> All =
    [
        new LegalBasisResponse(
            ResponsibleSecretariat.Health,
            "Art. 11, II, \"a\"",
            "Tratamento de dados sensíveis de saúde para tutela da saúde, em procedimento realizado por profissionais " +
            "de saúde ou por entidade sanitária, no âmbito do SUS municipal."),
        new LegalBasisResponse(
            ResponsibleSecretariat.Education,
            "Art. 7º, III",
            "Tratamento de dados necessário à execução de política pública prevista em lei ou regulamento — " +
            "atendimento educacional especializado e acompanhamento pedagógico da rede municipal de ensino."),
        new LegalBasisResponse(
            ResponsibleSecretariat.SocialAssistance,
            "Art. 7º, III",
            "Tratamento de dados necessário à execução de política pública de assistência social prevista em lei, " +
            "no âmbito do SUAS municipal, para acompanhamento familiar e encaminhamento da rede de proteção."),
    ];

    /// <inheritdoc />
    public Task<Result<IReadOnlyList<LegalBasisResponse>>> Handle(
        GetLegalBasisQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyList<LegalBasisResponse> response = request.Secretariat is { } secretariat
            ? All.Where(l => l.Secretariat == secretariat).ToList()
            : All;

        return Task.FromResult(Result<IReadOnlyList<LegalBasisResponse>>.Success(response));
    }
}
