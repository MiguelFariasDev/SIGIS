using FluentAssertions;
using Sigis.Domain.Entities;
using Sigis.Domain.ValueObjects;
using Sigis.Infrastructure.Persistence.Repositories;
using Sigis.Infrastructure.Tests.Fixtures;

namespace Sigis.Infrastructure.Tests.Persistence;

[Collection("Postgres")]
public class PersonRepositoryTests
{
    private readonly PostgresFixture _fixture;

    public PersonRepositoryTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    private static Person CreateTestPerson(string name, DateOnly birthDate, Cns? cns = null)
        => Person.Create(PersonName.Create(name).Value, birthDate, DateOnly.FromDateTime(DateTime.UtcNow), cns: cns).Value;

    [Fact]
    public async Task AddAsync_e_GetByIdAsync_devem_persistir_e_recuperar_a_pessoa()
    {
        await using var context = _fixture.CreateContext();
        var repository = new PersonRepository(context);
        var person = CreateTestPerson("Roberto Alves Cavalcante", new DateOnly(1990, 4, 12));

        await repository.AddAsync(person, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        await using var readContext = _fixture.CreateContext();
        var found = await new PersonRepository(readContext).GetByIdAsync(person.Id, CancellationToken.None);

        found.Should().NotBeNull();
        found!.Name.Value.Should().Be("Roberto Alves Cavalcante");
    }

    [Fact]
    public async Task GetByCnsAsync_deve_encontrar_pessoa_pelo_cns()
    {
        var cns = Cns.Create("200000000030018").Value;
        await using var context = _fixture.CreateContext();
        var repository = new PersonRepository(context);
        var person = CreateTestPerson("Fernanda Costa Ribeiro", new DateOnly(1985, 8, 20), cns);

        await repository.AddAsync(person, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        await using var readContext = _fixture.CreateContext();
        var found = await new PersonRepository(readContext).GetByCnsAsync(cns, CancellationToken.None);

        found.Should().NotBeNull();
        found!.Id.Should().Be(person.Id);
    }

    [Fact]
    public async Task Cns_deve_ser_unico_no_banco()
    {
        var cns = Cns.Create("200000000040005").Value;
        var personA = CreateTestPerson("Marcelo Souza Teixeira", new DateOnly(1978, 1, 5), cns);
        var personB = CreateTestPerson("Outra Pessoa Diferente", new DateOnly(1980, 2, 10), cns);

        await using (var context = _fixture.CreateContext())
        {
            await context.Persons.AddAsync(personA);
            await context.SaveChangesAsync();
        }

        await using var duplicateContext = _fixture.CreateContext();
        await duplicateContext.Persons.AddAsync(personB);

        var act = async () => await duplicateContext.SaveChangesAsync();

        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task SearchByNameAsync_deve_encontrar_pessoa_buscando_sem_acento()
    {
        await using var context = _fixture.CreateContext();
        var repository = new PersonRepository(context);
        var person = CreateTestPerson("José Antônio Conceição", new DateOnly(1995, 6, 15));

        await repository.AddAsync(person, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        await using var readContext = _fixture.CreateContext();
        var results = await new PersonRepository(readContext).SearchByNameAsync("Jose Antonio", CancellationToken.None);

        results.Should().Contain(p => p.Id == person.Id);
    }

    [Fact]
    public async Task SearchByNameAsync_deve_encontrar_pessoa_buscando_com_acento()
    {
        await using var context = _fixture.CreateContext();
        var repository = new PersonRepository(context);
        var person = CreateTestPerson("Conceição Aparecida Nogueira", new DateOnly(1992, 3, 3));

        await repository.AddAsync(person, CancellationToken.None);
        await context.SaveChangesAsync(CancellationToken.None);

        await using var readContext = _fixture.CreateContext();
        var results = await new PersonRepository(readContext).SearchByNameAsync("Conceição", CancellationToken.None);

        results.Should().Contain(p => p.Id == person.Id);
    }
}
