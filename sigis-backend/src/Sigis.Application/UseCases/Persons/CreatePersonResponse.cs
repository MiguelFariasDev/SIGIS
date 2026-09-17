namespace Sigis.Application.UseCases.Persons;

/// <summary>
/// Resposta do cadastro de uma pessoa. Quando <see cref="Candidates"/> não é
/// vazio, o cadastro foi criado normalmente, mas o sistema encontrou
/// possíveis duplicatas (RN02, camada 3 — probabilística) que exigem revisão
/// de um coordenador; um <c>DuplicateAlert</c> já foi aberto para cada
/// candidato. Cadastros nunca são bloqueados por suspeita de duplicidade —
/// apenas sinalizados — porque a fusão em si exige confirmação humana
/// (ver <c>DuplicateAlert</c>).
/// </summary>
/// <param name="PersonId">Identificador da pessoa recém-criada.</param>
/// <param name="Candidates">Candidatos a duplicidade encontrados, podendo ser vazio.</param>
public sealed record CreatePersonResponse(Guid PersonId, IReadOnlyList<PersonSummary> Candidates);
