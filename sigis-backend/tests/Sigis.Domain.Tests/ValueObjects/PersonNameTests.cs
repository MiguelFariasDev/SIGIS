using FluentAssertions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.ValueObjects;

namespace Sigis.Domain.Tests.ValueObjects;

public class PersonNameTests
{
    [Fact]
    public void Create_deve_ter_sucesso_com_nome_valido()
    {
        var result = PersonName.Create("Maria da Silva");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("Maria da Silva");
    }

    [Fact]
    public void Create_deve_normalizar_espacos_multiplos()
    {
        var result = PersonName.Create("Maria    da   Silva");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("Maria da Silva");
    }

    [Fact]
    public void Create_deve_normalizar_trim_nas_bordas()
    {
        var result = PersonName.Create("   Maria da Silva   ");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("Maria da Silva");
    }

    [Fact]
    public void Create_deve_gerar_normalized_sem_acento_e_minusculo()
    {
        var result = PersonName.Create("José António");

        result.IsSuccess.Should().BeTrue();
        result.Value.Normalized.Should().Be("jose antonio");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_deve_falhar_com_nome_vazio(string? rawName)
    {
        var result = PersonName.Create(rawName);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.NomeVazio);
    }

    [Fact]
    public void Create_deve_ter_sucesso_no_limite_minimo_de_5_caracteres()
    {
        var result = PersonName.Create("Jo Al");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_deve_falhar_com_menos_de_5_caracteres()
    {
        var result = PersonName.Create("Al");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.NomeMuitoCurto);
    }

    [Fact]
    public void Create_deve_falhar_com_mais_de_150_caracteres()
    {
        var longName = "Maria " + new string('a', 150);

        var result = PersonName.Create(longName);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.NomeMuitoLongo);
    }

    [Fact]
    public void Create_deve_falhar_com_apenas_uma_palavra()
    {
        var result = PersonName.Create("Mariazinha");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.NomeUmaPalavra);
    }

    [Fact]
    public void Create_deve_falhar_quando_alguma_palavra_tem_menos_de_2_caracteres()
    {
        var result = PersonName.Create("Maria A");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.NomePalavraCurta);
    }

    [Theory]
    [InlineData("Maria123 Silva")]
    [InlineData("Maria@ Silva")]
    [InlineData("12345 67890")]
    public void Create_deve_falhar_com_caracteres_invalidos(string rawName)
    {
        var result = PersonName.Create(rawName);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.NomeInvalido);
    }

    [Fact]
    public void Create_deve_aceitar_hifen_e_apostrofo_no_nome()
    {
        var result = PersonName.Create("Maria D'Ávila-Souza");

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void SimilarityTo_deve_retornar_1_para_nomes_identicos()
    {
        var nameA = PersonName.Create("Maria da Silva").Value;
        var nameB = PersonName.Create("Maria da Silva").Value;

        nameA.SimilarityTo(nameB).Should().Be(1d);
    }

    [Fact]
    public void SimilarityTo_deve_ser_alta_para_nomes_muito_parecidos()
    {
        var nameA = PersonName.Create("Maria da Silva Souza").Value;
        var nameB = PersonName.Create("Maria da Silva Sousa").Value;

        nameA.SimilarityTo(nameB).Should().BeGreaterThan(0.75);
    }

    [Fact]
    public void SimilarityTo_deve_ser_baixa_para_nomes_diferentes()
    {
        var nameA = PersonName.Create("Maria da Silva").Value;
        var nameB = PersonName.Create("Joao Pereira Neto").Value;

        nameA.SimilarityTo(nameB).Should().BeLessThan(0.3);
    }

    [Fact]
    public void SimilarityTo_deve_lancar_ArgumentNullException_quando_other_e_nulo()
    {
        var name = PersonName.Create("Maria da Silva").Value;

        var act = () => name.SimilarityTo(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}
