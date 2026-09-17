using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Sigis.Infrastructure.Tests.Fixtures;

namespace Sigis.Infrastructure.Tests.Extensions;

/// <summary>
/// Testes das extensões PostgreSQL <c>pg_trgm</c> e <c>unaccent</c>
/// diretamente via SQL, validando que estão instaladas e se comportam como
/// o SIGIS espera (usadas pela deduplicação e pela busca por nome).
/// </summary>
[Collection("Postgres")]
public class TrigramExtensionTests
{
    private readonly PostgresFixture _fixture;

    public TrigramExtensionTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<T> ScalarAsync<T>(string sql)
    {
        await using var context = _fixture.CreateContext();
        await using var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = sql;

        var result = await command.ExecuteScalarAsync();
        return (T)result!;
    }

    [Fact]
    public async Task Similarity_de_strings_identicas_deve_ser_1()
    {
        var score = await ScalarAsync<float>("SELECT similarity('maria', 'maria');");

        score.Should().Be(1.0f);
    }

    [Fact]
    public async Task Similarity_deve_ser_pelo_menos_0_5_para_substring_relacionada()
    {
        var score = await ScalarAsync<float>("SELECT similarity('maria', 'maria silva');");

        score.Should().BeGreaterThanOrEqualTo(0.5f);
    }

    [Fact]
    public async Task Similarity_deve_ser_baixa_para_strings_nao_relacionadas()
    {
        var score = await ScalarAsync<float>("SELECT similarity('maria', 'joao pereira');");

        score.Should().BeLessThan(0.3f);
    }

    [Fact]
    public async Task Unaccent_deve_remover_acentuacao()
    {
        var result = await ScalarAsync<string>("SELECT unaccent('José');");

        result.Should().Be("Jose");
    }

    [Fact]
    public async Task ImmutableUnaccentWrapper_deve_produzir_o_mesmo_resultado_que_unaccent()
    {
        var result = await ScalarAsync<string>("SELECT sigis.immutable_unaccent(lower('José da Conceição'));");

        result.Should().Be("jose da conceicao");
    }
}
