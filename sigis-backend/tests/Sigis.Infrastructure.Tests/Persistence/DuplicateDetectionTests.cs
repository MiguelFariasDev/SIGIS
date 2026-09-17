using FluentAssertions;
using Sigis.Domain.Entities;
using Sigis.Domain.ValueObjects;
using Sigis.Infrastructure.Persistence.Repositories;
using Sigis.Infrastructure.Tests.Fixtures;

namespace Sigis.Infrastructure.Tests.Persistence;

/// <summary>
/// Testes da camada 3 de deduplicação (RN02): matching probabilístico via
/// <c>pg_trgm</c>, com filtro duro por data de nascimento e limiar de
/// similaridade de 0,75 — ver <see cref="DuplicateAlert.MinimumSimilarityThreshold"/>.
/// </summary>
[Collection("Postgres")]
public class DuplicateDetectionTests
{
    private readonly PostgresFixture _fixture;

    public DuplicateDetectionTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    private static Person CreateTestPerson(string name, DateOnly birthDate)
        => Person.Create(PersonName.Create(name).Value, birthDate, DateOnly.FromDateTime(DateTime.UtcNow)).Value;

    [Fact]
    public async Task FindDuplicateCandidatesAsync_deve_retornar_match_exato_de_nome_e_data()
    {
        var birthDate = new DateOnly(2001, 7, 19);
        var original = CreateTestPerson("Henrique Barbosa Almeida Junior", birthDate);

        await using (var context = _fixture.CreateContext())
        {
            await context.Persons.AddAsync(original);
            await context.SaveChangesAsync();
        }

        await using var readContext = _fixture.CreateContext();
        var repository = new PersonRepository(readContext);
        var searchName = PersonName.Create("Henrique Barbosa Almeida Junior").Value;

        var candidates = await repository.FindDuplicateCandidatesAsync(searchName, birthDate, CancellationToken.None);

        candidates.Should().Contain(p => p.Id == original.Id);
    }

    [Fact]
    public async Task FindDuplicateCandidatesAsync_deve_retornar_candidato_com_score_acima_do_limiar()
    {
        var birthDate = new DateOnly(2002, 9, 30);
        var original = CreateTestPerson("Wellington Pereira dos Santos", birthDate);

        await using (var context = _fixture.CreateContext())
        {
            await context.Persons.AddAsync(original);
            await context.SaveChangesAsync();
        }

        await using var readContext = _fixture.CreateContext();
        var repository = new PersonRepository(readContext);

        // Pequena variação ortográfica ("dos Santos" -> "do Santos") — deve
        // permanecer acima do limiar de 0.75 de similaridade por trigrama.
        var similarName = PersonName.Create("Wellington Pereira do Santos").Value;

        var candidates = await repository.FindDuplicateCandidatesAsync(similarName, birthDate, CancellationToken.None);

        candidates.Should().Contain(p => p.Id == original.Id);
    }

    [Fact]
    public async Task FindDuplicateCandidatesAsync_nao_deve_retornar_candidato_com_score_abaixo_do_limiar()
    {
        var birthDate = new DateOnly(2003, 11, 11);
        var original = CreateTestPerson("Alexandre Nunes Vasconcelos", birthDate);

        await using (var context = _fixture.CreateContext())
        {
            await context.Persons.AddAsync(original);
            await context.SaveChangesAsync();
        }

        await using var readContext = _fixture.CreateContext();
        var repository = new PersonRepository(readContext);

        // Nome completamente diferente — score de similaridade deve ficar
        // bem abaixo do limiar de 0.75.
        var unrelatedName = PersonName.Create("Patricia Gomes Ferreira").Value;

        var candidates = await repository.FindDuplicateCandidatesAsync(unrelatedName, birthDate, CancellationToken.None);

        candidates.Should().NotContain(p => p.Id == original.Id);
    }

    [Fact]
    public async Task FindDuplicateCandidatesAsync_nao_deve_retornar_candidato_com_data_de_nascimento_diferente()
    {
        var original = CreateTestPerson("Cristiane Rodrigues Martins", new DateOnly(1999, 4, 4));

        await using (var context = _fixture.CreateContext())
        {
            await context.Persons.AddAsync(original);
            await context.SaveChangesAsync();
        }

        await using var readContext = _fixture.CreateContext();
        var repository = new PersonRepository(readContext);
        var sameName = PersonName.Create("Cristiane Rodrigues Martins").Value;

        // Mesmo nome exato, mas data de nascimento diferente — o filtro duro
        // por data (RN02) deve excluir este candidato, mesmo com score 1.0.
        var candidates = await repository.FindDuplicateCandidatesAsync(
            sameName, new DateOnly(1999, 4, 5), CancellationToken.None);

        candidates.Should().NotContain(p => p.Id == original.Id);
    }
}
