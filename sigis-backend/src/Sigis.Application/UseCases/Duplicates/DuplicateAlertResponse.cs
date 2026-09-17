namespace Sigis.Application.UseCases.Duplicates;

/// <summary>Resumo de um alerta de duplicidade pendente de revisão.</summary>
/// <param name="Id">Identificador do alerta.</param>
/// <param name="PersonId1">Identificador do primeiro cadastro envolvido.</param>
/// <param name="PersonName1">Nome do primeiro cadastro envolvido.</param>
/// <param name="PersonId2">Identificador do segundo cadastro envolvido.</param>
/// <param name="PersonName2">Nome do segundo cadastro envolvido.</param>
/// <param name="SimilarityScore">Score de similaridade entre os dois cadastros (0 a 1).</param>
/// <param name="CreatedAt">Data e hora (UTC) de criação do alerta.</param>
public sealed record DuplicateAlertResponse(
    Guid Id,
    Guid PersonId1,
    string PersonName1,
    Guid PersonId2,
    string PersonName2,
    double SimilarityScore,
    DateTime CreatedAt);
