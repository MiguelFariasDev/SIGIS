namespace Sigis.Infrastructure.Seed.SeedData;

/// <summary>
/// Dado estático de uma pessoa a ser semeada. Campos de valor bruto (CNS/CPF
/// em dígitos, endereço em partes) são validados e convertidos para os
/// respectivos Value Objects pelo <c>PersonSeeder</c>.
/// </summary>
/// <param name="Key">Chave curta de referência interna do seed (ex.: "pedro"), usada pelos demais seeders para localizar esta pessoa.</param>
/// <param name="Name">Nome completo.</param>
/// <param name="BirthDate">Data de nascimento.</param>
/// <param name="MotherName">Nome da mãe, opcional.</param>
/// <param name="CnsDigits">Número do CNS (15 dígitos, já validado contra o dígito verificador), opcional.</param>
/// <param name="CpfDigits">Número do CPF (11 dígitos), opcional.</param>
/// <param name="AddressStreet">Logradouro do endereço, opcional (os demais campos de endereço só se aplicam quando este for informado).</param>
/// <param name="AddressNumber">Número do endereço.</param>
/// <param name="AddressNeighborhood">Bairro do endereço.</param>
/// <param name="AddressCity">Cidade do endereço.</param>
/// <param name="AddressState">UF do endereço.</param>
/// <param name="AddressZip">CEP do endereço.</param>
/// <param name="DisabilityTypes">Tipos de deficiência/transtorno, separados por vírgula, opcional (NAPE A.4).</param>
public sealed record PersonSeed(
    string Key,
    string Name,
    DateOnly BirthDate,
    string? MotherName = null,
    string? CnsDigits = null,
    string? CpfDigits = null,
    string? AddressStreet = null,
    string? AddressNumber = null,
    string? AddressNeighborhood = null,
    string? AddressCity = null,
    string? AddressState = null,
    string? AddressZip = null,
    string? DisabilityTypes = null);

/// <summary>
/// As 12 pessoas de demonstração: 5 casos nomeados (gêmeas de cadastro,
/// timeline federada, consentimento parcial, busca ativa) + 7 perfis comuns.
/// </summary>
public static class PersonSeedData
{
    /// <summary>Pessoas a semear.</summary>
    public static readonly IReadOnlyList<PersonSeed> Persons =
    [
        // Caso 1/2 — gêmeas de cadastro (mesmo nome, mesma data de nascimento,
        // mesmo endereço) — aciona a camada 3 de deduplicação (RN02).
        new("twin1", "Maria Silva Santos", new DateOnly(2018, 5, 10), MotherName: "Ana Paula Santos",
            AddressStreet: "Rua das Flores", AddressNumber: "123", AddressNeighborhood: "Centro",
            AddressCity: "Crateús", AddressState: "CE", AddressZip: "63700-000", DisabilityTypes: "TEA"),
        new("twin2", "Maria Silva Santos", new DateOnly(2018, 5, 10), MotherName: "Ana Paula Santos",
            AddressStreet: "Rua das Flores", AddressNumber: "123", AddressNeighborhood: "Centro",
            AddressCity: "Crateús", AddressState: "CE", AddressZip: "63700-000", DisabilityTypes: "TEA"),

        // Caso 3 — timeline federada (histórico em NASF, NAPE e CRASF).
        new("pedro", "Pedro Henrique Oliveira", new DateOnly(2016, 2, 15), MotherName: "Juliana Oliveira",
            CnsDigits: "929141777631704",
            AddressStreet: "Av. Central", AddressNumber: "456", AddressNeighborhood: "Bairro São José",
            AddressCity: "Crateús", AddressState: "CE", AddressZip: "63700-000", DisabilityTypes: "TEA, TDAH"),

        // Caso 4 — consentimento parcial (SocialAssistance nunca concedido) bloqueia o encaminhamento para o CRASF.
        new("anaClara", "Ana Clara Pereira", new DateOnly(2020, 11, 8), MotherName: "Fernanda Pereira",
            CnsDigits: "901152449390922", DisabilityTypes: "TEA"),

        // Caso 5 — 2 faltas consecutivas → busca ativa (RN04).
        new("lucas", "Lucas Gabriel Souza", new DateOnly(2019, 7, 22), MotherName: "Patrícia Souza",
            CnsDigits: "939825979190746", DisabilityTypes: "TEA"),

        // Perfis comuns (7): reaproveitam nomes/documentos já validados do
        // seed original, para manter dados historicamente estáveis.
        new("joaoPedro", "João Pedro Alves Santos", new DateOnly(2016, 3, 12), CnsDigits: "100000000010002",
            DisabilityTypes: "TEA"),
        new("mariaEduarda", "Maria Eduarda Costa Lima", new DateOnly(2017, 7, 4), CnsDigits: "100000000020008",
            DisabilityTypes: "TEA"),
        new("luizGustavo", "Luiz Gustavo Pereira Souza", new DateOnly(2014, 11, 30), CpfDigits: "52998224725",
            DisabilityTypes: "TDAH"),
        new("sofiaVitoria", "Sofia Vitória Barbosa Nunes", new DateOnly(2019, 9, 2), CnsDigits: "100000000040009"),
        new("rafael", "Rafael Augusto Araújo Lima", new DateOnly(2003, 8, 8), CnsDigits: "934167211068400",
            DisabilityTypes: "TEA"),
        new("isabelly", "Isabelly Cristina Moura Rocha", new DateOnly(2016, 12, 25), CnsDigits: "100000000050004",
            DisabilityTypes: "TEA, Deficiência Física"),
        new("gabriel", "Gabriel da Silva Nascimento", new DateOnly(2013, 4, 17), CnsDigits: "994580730215734"),
    ];
}
