using FluentAssertions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.ValueObjects;

namespace Sigis.Domain.Tests.ValueObjects;

public class CnsTests
{
    // CNS definitivo válido: PIS "12345678901" com dígito verificador calculado
    // pelo algoritmo oficial (soma ponderada dos 11 primeiros dígitos ≡ 0 mod 11).
    private const string ValidDefinitiveCns = "123456789010000";

    // CNS provisório válido (inicia com 7): soma ponderada dos 15 dígitos ≡ 0 mod 11.
    private const string ValidProvisionalCns = "700000000000005";

    [Fact]
    public void Create_deve_ter_sucesso_com_cns_definitivo_valido()
    {
        var result = Cns.Create(ValidDefinitiveCns);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(ValidDefinitiveCns);
    }

    [Fact]
    public void Create_deve_ter_sucesso_com_cns_provisorio_valido()
    {
        var result = Cns.Create(ValidProvisionalCns);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(ValidProvisionalCns);
    }

    [Fact]
    public void Create_deve_remover_caracteres_nao_numericos_antes_de_validar()
    {
        var masked = "123.456.789.0100-00"; // mesmos dígitos de ValidDefinitiveCns

        var result = Cns.Create(masked);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(ValidDefinitiveCns);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12345678901")] // menos de 15 dígitos
    [InlineData("1234567890123456")] // mais de 15 dígitos
    public void Create_deve_falhar_com_formato_invalido(string? rawCns)
    {
        var result = Cns.Create(rawCns);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.CnsInvalido);
    }

    [Fact]
    public void Create_deve_falhar_com_digito_verificador_incorreto()
    {
        var invalid = "123456789010001"; // último dígito alterado

        var result = Cns.Create(invalid);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.CnsInvalido);
    }

    [Fact]
    public void Create_deve_falhar_quando_primeiro_digito_nao_indica_tipo_conhecido()
    {
        var invalid = "323456789010000"; // inicia com 3 — não é definitivo (1/2) nem provisório (7/8/9)

        var result = Cns.Create(invalid);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.CnsInvalido);
    }
}
