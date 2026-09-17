using FluentAssertions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.ValueObjects;

namespace Sigis.Domain.Tests.ValueObjects;

public class EmailAddressTests
{
    [Fact]
    public void Create_deve_ter_sucesso_com_email_valido()
    {
        var result = EmailAddress.Create("Profissional@Saude.Crateus.CE.Gov.Br");

        result.IsSuccess.Should().BeTrue();
        result.Value.Value.Should().Be("profissional@saude.crateus.ce.gov.br");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("nao-e-email")]
    [InlineData("@sememail.com")]
    [InlineData("usuario@")]
    public void Create_deve_falhar_com_email_invalido(string? rawEmail)
    {
        var result = EmailAddress.Create(rawEmail);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.EmailInvalido);
    }

    [Fact]
    public void Create_deve_falhar_quando_email_excede_254_caracteres()
    {
        var longLocalPart = new string('a', 250);
        var tooLong = $"{longLocalPart}@ex.com";

        var result = EmailAddress.Create(tooLong);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.EmailInvalido);
    }
}
