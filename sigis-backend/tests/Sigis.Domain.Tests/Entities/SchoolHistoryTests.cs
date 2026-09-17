using FluentAssertions;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;

namespace Sigis.Domain.Tests.Entities;

public class SchoolHistoryTests
{
    private static Result<SchoolHistory> CreateSample(int schoolYear = 2026, DateOnly? startDate = null)
        => SchoolHistory.Create(
            Guid.NewGuid(), "Escola Municipal Dom Pedro", "5º ano", schoolYear, Guid.NewGuid(),
            DateTime.UtcNow, currentYear: 2026, startDate: startDate);

    [Fact]
    public void Create_deve_ter_sucesso_com_status_inicial_ativo()
    {
        var result = CreateSample();

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(SchoolStatus.Ativo);
        result.Value.IsActive().Should().BeTrue();
    }

    [Fact]
    public void Create_deve_falhar_sem_nome_da_escola()
    {
        var result = SchoolHistory.Create(Guid.NewGuid(), null, "5º ano", 2026, Guid.NewGuid(), DateTime.UtcNow, 2026);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SchoolHistoryErrors.NomeEscolaObrigatorio);
    }

    [Fact]
    public void Create_deve_falhar_sem_serie()
    {
        var result = SchoolHistory.Create(
            Guid.NewGuid(), "Escola Municipal Dom Pedro", null, 2026, Guid.NewGuid(), DateTime.UtcNow, 2026);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SchoolHistoryErrors.SerieObrigatoria);
    }

    [Theory]
    [InlineData(1899)]
    [InlineData(2027)]
    public void Create_deve_falhar_com_ano_letivo_fora_do_intervalo(int schoolYear)
    {
        var result = CreateSample(schoolYear);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SchoolHistoryErrors.AnoLetivoInvalido);
    }

    [Fact]
    public void Close_deve_falhar_ao_informar_data_de_termino_com_status_ativo()
    {
        var schoolHistory = CreateSample().Value;

        var result = schoolHistory.Close(SchoolStatus.Ativo, DateOnly.FromDateTime(DateTime.UtcNow));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SchoolHistoryErrors.DataTerminoApenasQuandoInativo);
    }

    [Fact]
    public void Close_deve_falhar_quando_data_de_termino_e_anterior_ao_inicio()
    {
        var startDate = new DateOnly(2026, 6, 1);
        var schoolHistory = CreateSample(startDate: startDate).Value;

        var result = schoolHistory.Close(SchoolStatus.Transferido, startDate.AddDays(-1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(SchoolHistoryErrors.DataTerminoInvalida);
    }

    [Fact]
    public void Close_deve_ter_sucesso_e_atualizar_status_e_data_de_termino()
    {
        var startDate = new DateOnly(2026, 2, 1);
        var schoolHistory = CreateSample(startDate: startDate).Value;

        var result = schoolHistory.Close(SchoolStatus.Concluido, startDate.AddMonths(10));

        result.IsSuccess.Should().BeTrue();
        schoolHistory.Status.Should().Be(SchoolStatus.Concluido);
        schoolHistory.IsActive().Should().BeFalse();
    }
}
