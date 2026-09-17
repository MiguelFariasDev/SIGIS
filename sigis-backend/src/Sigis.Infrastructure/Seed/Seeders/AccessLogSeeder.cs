using Sigis.Domain.Entities;

namespace Sigis.Infrastructure.Seed.Seeders;

/// <summary>Semeia os logs de auditoria de acesso cross-secretaria/cross-unidade (5 no total).</summary>
public static class AccessLogSeeder
{
    // access_log.legal_basis é varchar(50) — manter curto (ver nota em
    // RequestPersonAccessCommandHandler/GetPersonTimelineQueryHandler, que já
    // sofreram overflow com um texto mais longo).
    private const string LegalBasis = "LGPD art. 11, II — políticas públicas";

    /// <summary>
    /// Cria 5 registros de auditoria de acesso entre unidades/secretarias
    /// diferentes, incluindo o acesso de Camila (NASF) à timeline federada
    /// de Pedro Henrique (caso 3).
    /// </summary>
    /// <param name="persons">Pessoas já semeadas, indexadas pela chave curta do seed.</param>
    /// <param name="professionals">Profissionais já semeados, indexados pela chave curta do seed.</param>
    /// <param name="now">Instante de referência (UTC) dos acessos.</param>
    /// <returns>Os registros de auditoria criados.</returns>
    public static List<AccessLog> Seed(
        IReadOnlyDictionary<string, Person> persons, IReadOnlyDictionary<string, Professional> professionals, DateTime now)
    {
        var logs = new List<AccessLog>
        {
            AccessLog.Create(
                persons["pedro"].Id, professionals["coordenador"].Id, "VisualizacaoTimeline", LegalBasis,
                now.AddDays(-1), isCrossUnit: true, justification: "Coordenação de cuidado intersetorial").Value,

            AccessLog.Create(
                persons["twin1"].Id, professionals["psicologo"].Id, "SolicitacaoAcesso", LegalBasis,
                now.AddDays(-2), isCrossUnit: true, justification: "Continuidade pedagógica do caso").Value,

            AccessLog.Create(
                persons["anaClara"].Id, professionals["assistente"].Id, "VisualizacaoTimeline", LegalBasis,
                now.AddDays(-3), isCrossUnit: true, justification: "Avaliação para encaminhamento socioassistencial").Value,

            AccessLog.Create(
                persons["lucas"].Id, professionals["fono"].Id, "VisualizacaoTimeline", LegalBasis,
                now.AddDays(-4), isCrossUnit: true, justification: "Preparação para atendimento fonoaudiológico").Value,

            AccessLog.Create(
                persons["twin2"].Id, professionals["coordenador"].Id, "SolicitacaoAcesso", LegalBasis,
                now.AddHours(-2), isCrossUnit: true, justification: "Revisão de alerta de duplicidade").Value,
        };

        return logs;
    }
}
