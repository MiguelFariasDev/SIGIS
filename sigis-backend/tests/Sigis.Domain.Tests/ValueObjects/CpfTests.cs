using FluentAssertions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.ValueObjects;

namespace Sigis.Domain.Tests.ValueObjects;

public class CpfTests
{
    private const string ValidCpf = "52998224725";

    [Fact]
    public void Create_deve_ter_sucesso_com_cpf_valido()
    {
        var result = Cpf.Create(ValidCpf);

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(ValidCpf);
    }

    [Fact]
    public void Create_deve_remover_mascara_antes_de_validar()
    {
        var result = Cpf.Create("529.982.247-25");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be(ValidCpf);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("52998224724")] // dígito verificador incorreto
    public void Create_deve_falhar_com_cpf_invalido(string? rawCpf)
    {
        var result = Cpf.Create(rawCpf);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.CpfInvalido);
    }

    [Theory]
    [InlineData("11111111111")]
    [InlineData("00000000000")]
    [InlineData("99999999999")]
    public void Create_deve_falhar_com_sequencia_de_digitos_repetidos(string rawCpf)
    {
        var result = Cpf.Create(rawCpf);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.CpfInvalido);
    }

    [Fact]
    public void Formatted_deve_retornar_cpf_com_mascara()
    {
        var cpf = Cpf.Create(ValidCpf).Value;

        cpf.Formatted().Should().Be("529.982.247-25");
    }
}
