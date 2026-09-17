using FluentAssertions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.ValueObjects;

namespace Sigis.Domain.Tests.ValueObjects;

public class AddressTests
{
    [Fact]
    public void Create_deve_ter_sucesso_com_todos_os_campos_obrigatorios()
    {
        var result = Address.Create("Rua Tal", "123", "Centro", "Crateús", "ce", "63700-000");

        result.IsSuccess.Should().BeTrue();
        result.Value.State.Should().Be("CE");
    }

    [Fact]
    public void Create_deve_ter_sucesso_sem_cep_informado()
    {
        var result = Address.Create("Rua Tal", "123", "Centro", "Crateús", "CE");

        result.IsSuccess.Should().BeTrue();
        result.Value.ZipCode.Should().BeNull();
    }

    [Theory]
    [InlineData(null, "123", "Centro", "Crateús", "CE")]
    [InlineData("Rua Tal", null, "Centro", "Crateús", "CE")]
    [InlineData("Rua Tal", "123", null, "Crateús", "CE")]
    [InlineData("Rua Tal", "123", "Centro", null, "CE")]
    [InlineData("Rua Tal", "123", "Centro", "Crateús", null)]
    [InlineData("Rua Tal", "123", "Centro", "Crateús", "C")]
    [InlineData("Rua Tal", "123", "Centro", "Crateús", "CEA")]
    public void Create_deve_falhar_quando_campo_obrigatorio_ausente_ou_uf_invalida(
        string? street, string? number, string? neighborhood, string? city, string? state)
    {
        var result = Address.Create(street, number, neighborhood, city, state);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.EnderecoInvalido);
    }

    [Fact]
    public void Full_deve_formatar_endereco_completo_com_cep()
    {
        var address = Address.Create("Rua Tal", "123", "Centro", "Crateús", "CE", "63700-000").Value;

        address.Full().Should().Be("Rua Tal, 123, Centro, Crateús/CE - CEP 63700-000");
    }

    [Fact]
    public void Full_deve_formatar_endereco_sem_cep()
    {
        var address = Address.Create("Rua Tal", "123", "Centro", "Crateús", "CE").Value;

        address.Full().Should().Be("Rua Tal, 123, Centro, Crateús/CE");
    }
}
