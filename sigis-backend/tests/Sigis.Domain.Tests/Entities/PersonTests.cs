using FluentAssertions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Entities;
using Sigis.Domain.Events;
using Sigis.Domain.ValueObjects;

namespace Sigis.Domain.Tests.Entities;

public class PersonTests
{
    private static readonly PersonName SampleName = PersonName.Create("Maria da Silva").Value;
    private static readonly DateOnly Today = new(2026, 9, 16);

    [Fact]
    public void Create_deve_ter_sucesso_com_dados_minimos()
    {
        var result = Person.Create(SampleName, new DateOnly(2015, 5, 10), Today);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(SampleName);
        result.Value.Guardians.Should().BeEmpty();
    }

    [Fact]
    public void Create_deve_emitir_PersonRegisteredEvent()
    {
        var result = Person.Create(SampleName, new DateOnly(2015, 5, 10), Today);

        var person = result.Value;

        person.Events.Should().ContainSingle()
            .Which.Should().BeOfType<PersonRegisteredEvent>()
            .Which.PersonId.Should().Be(person.Id);
    }

    [Fact]
    public void Create_deve_falhar_com_data_de_nascimento_futura()
    {
        var result = Person.Create(SampleName, Today.AddDays(1), Today);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.DataNascimentoFutura);
    }

    [Fact]
    public void Create_deve_falhar_com_data_de_nascimento_anterior_a_1900()
    {
        var result = Person.Create(SampleName, new DateOnly(1899, 12, 31), Today);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.DataNascimentoMuitoAntiga);
    }

    [Fact]
    public void CalculateAge_deve_calcular_idade_em_anos_completos()
    {
        var person = Person.Create(SampleName, new DateOnly(2015, 9, 20), Today).Value;

        // Aniversário em 20/09 ainda não ocorreu em 16/09/2026 -> 10 anos, não 11.
        person.CalculateAge(Today).Should().Be(10);
    }

    [Fact]
    public void CalculateAge_deve_considerar_aniversario_ja_ocorrido_no_ano()
    {
        var person = Person.Create(SampleName, new DateOnly(2015, 1, 1), Today).Value;

        person.CalculateAge(Today).Should().Be(11);
    }

    [Fact]
    public void HasValidCns_deve_retornar_false_quando_cns_nao_informado()
    {
        var person = Person.Create(SampleName, new DateOnly(2015, 5, 10), Today).Value;

        person.HasValidCns().Should().BeFalse();
    }

    [Fact]
    public void HasValidCns_deve_retornar_true_quando_cns_informado()
    {
        var cns = Cns.Create("123456789010000").Value;
        var person = Person.Create(SampleName, new DateOnly(2015, 5, 10), Today, cns: cns).Value;

        person.HasValidCns().Should().BeTrue();
    }

    [Fact]
    public void DemographicKey_deve_combinar_nome_normalizado_e_data_de_nascimento()
    {
        var person = Person.Create(SampleName, new DateOnly(2015, 5, 10), Today).Value;

        person.DemographicKey().Should().Be("maria da silva|2015-05-10");
    }

    [Fact]
    public void AddGuardian_deve_ter_sucesso_e_vincular_responsavel()
    {
        var person = Person.Create(SampleName, new DateOnly(2015, 5, 10), Today).Value;

        var result = person.AddGuardian(SampleName, "Mãe");

        result.IsSuccess.Should().BeTrue();
        person.Guardians.Should().ContainSingle(g => g.Relationship == "Mãe");
    }

    [Fact]
    public void AddGuardian_deve_falhar_sem_relacionamento()
    {
        var person = Person.Create(SampleName, new DateOnly(2015, 5, 10), Today).Value;

        var result = person.AddGuardian(SampleName, relationship: null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.RelacionamentoObrigatorio);
        person.Guardians.Should().BeEmpty();
    }

    [Fact]
    public void RemoveGuardian_deve_remover_responsavel_existente()
    {
        var person = Person.Create(SampleName, new DateOnly(2015, 5, 10), Today).Value;
        person.AddGuardian(SampleName, "Mãe");
        var guardianId = person.Guardians.Single().Id;

        var result = person.RemoveGuardian(guardianId);

        result.IsSuccess.Should().BeTrue();
        person.Guardians.Should().BeEmpty();
    }

    [Fact]
    public void RemoveGuardian_deve_falhar_quando_responsavel_nao_existe()
    {
        var person = Person.Create(SampleName, new DateOnly(2015, 5, 10), Today).Value;

        var result = person.RemoveGuardian(Guid.NewGuid());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(PersonErrors.ResponsavelNaoEncontrado);
    }

    [Fact]
    public void Guardians_nao_deve_ser_modificavel_externamente()
    {
        var person = Person.Create(SampleName, new DateOnly(2015, 5, 10), Today).Value;

        person.Guardians.Should().BeAssignableTo<IReadOnlyCollection<Guardian>>();
    }
}
