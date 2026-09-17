using FluentAssertions;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Infrastructure.Persistence.Repositories;
using Sigis.Infrastructure.Tests.Fixtures;

namespace Sigis.Infrastructure.Tests.Persistence;

/// <summary>
/// Testes de RN01 (unicidade de fila ativa por pessoa e unidade) contra o
/// índice único parcial real do PostgreSQL (<c>idx_queue_active_unique</c>).
/// </summary>
[Collection("Postgres")]
public class QueueEntryRepositoryTests
{
    private readonly PostgresFixture _fixture;

    public QueueEntryRepositoryTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Duas_filas_ativas_para_mesma_pessoa_e_unidade_devem_violar_indice_unico()
    {
        var personId = Guid.NewGuid();
        var unitId = Guid.NewGuid();

        var firstEntry = QueueEntry.Create(personId, unitId, "Psicologia", QueuePriority.ShortTerm, DateTime.UtcNow).Value;
        var secondEntry = QueueEntry.Create(personId, unitId, "Psicologia", QueuePriority.Urgent, DateTime.UtcNow).Value;

        await using (var context = _fixture.CreateContext())
        {
            var repository = new QueueEntryRepository(context);
            await repository.AddAsync(firstEntry, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using var secondContext = _fixture.CreateContext();
        var secondRepository = new QueueEntryRepository(secondContext);
        await secondRepository.AddAsync(secondEntry, CancellationToken.None);

        var act = async () => await secondContext.SaveChangesAsync(CancellationToken.None);

        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task Fila_ativa_e_fila_concluida_para_mesma_pessoa_e_unidade_devem_coexistir()
    {
        var personId = Guid.NewGuid();
        var unitId = Guid.NewGuid();

        var completedEntry = QueueEntry.Create(
            personId, unitId, "Psicopedagogia", QueuePriority.ShortTerm, DateTime.UtcNow.AddDays(-10)).Value;
        completedEntry.MarkAsAttended();

        var activeEntry = QueueEntry.Create(
            personId, unitId, "Psicopedagogia", QueuePriority.Urgent, DateTime.UtcNow).Value;

        await using var context = _fixture.CreateContext();
        var repository = new QueueEntryRepository(context);

        await repository.AddAsync(completedEntry, CancellationToken.None);
        await repository.AddAsync(activeEntry, CancellationToken.None);

        var act = async () => await context.SaveChangesAsync(CancellationToken.None);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task HasActiveEntryAsync_deve_refletir_o_indice_unico_parcial()
    {
        var personId = Guid.NewGuid();
        var unitId = Guid.NewGuid();
        var entry = QueueEntry.Create(personId, unitId, "Terapia Ocupacional", QueuePriority.WaitingList, DateTime.UtcNow).Value;

        await using (var context = _fixture.CreateContext())
        {
            var repository = new QueueEntryRepository(context);
            await repository.AddAsync(entry, CancellationToken.None);
            await context.SaveChangesAsync(CancellationToken.None);
        }

        await using var readContext = _fixture.CreateContext();
        var hasActive = await new QueueEntryRepository(readContext)
            .HasActiveEntryAsync(personId, unitId, CancellationToken.None);

        hasActive.Should().BeTrue();
    }
}
