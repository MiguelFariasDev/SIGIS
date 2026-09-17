using FluentAssertions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.ValueObjects;

namespace Sigis.Domain.Tests.ValueObjects;

public class PhoneNumberTests
{
    [Fact]
    public void Create_deve_ter_sucesso_com_celular_valido()
    {
        var result = PhoneNumber.Create("(88) 99999-8888");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("+5588999998888");
    }

    [Fact]
    public void Create_deve_ter_sucesso_com_fixo_valido()
    {
        var result = PhoneNumber.Create("(88) 3221-1234");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("+558832211234");
    }

    [Fact]
    public void Create_deve_ter_sucesso_com_numero_ja_em_e164()
    {
        var result = PhoneNumber.Create("+5588999998888");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("+5588999998888");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("abc")]
    public void Create_deve_falhar_com_telefone_invalido(string? rawPhoneNumber)
    {
        var result = PhoneNumber.Create(rawPhoneNumber);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.TelefoneInvalido);
    }

    [Fact]
    public void Formatted_deve_retornar_numero_no_padrao_nacional()
    {
        var phone = PhoneNumber.Create("88999998888").Value;

        phone.Formatted().Should().Be("(88) 99999-8888");
    }
}
