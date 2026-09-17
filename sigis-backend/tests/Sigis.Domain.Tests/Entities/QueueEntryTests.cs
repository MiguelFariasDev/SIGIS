using FluentAssertions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;

namespace Sigis.Domain.Tests.Entities;

public class QueueEntryTests
{
    private static QueueEntry CreateEntry(QueuePriority priority = QueuePriority.ShortTerm) =>
        QueueEntry.Create(Guid.NewGuid(), Guid.NewGuid(), "Psicologia", priority, DateTime.UtcNow).Value;

    [Fact]
    public void Create_deve_ter_sucesso_com_status_inicial_waiting()
    {
        var entry = CreateEntry();

        entry.Status.Should().Be(QueueStatus.Waiting);
        entry.IsActive().Should().BeTrue();
    }

    [Fact]
    public void Create_deve_falhar_sem_especialidade()
    {
        var result = QueueEntry.Create(Guid.NewGuid(), Guid.NewGuid(), null, QueuePriority.Urgent, DateTime.UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(QueueErrors.EspecialidadeObrigatoria);
    }

    [Fact]
    public void CallForAttendance_deve_transicionar_para_in_attendance()
    {
        var entry = CreateEntry();

        var result = entry.CallForAttendance();

        result.IsSuccess.Should().BeTrue();
        entry.Status.Should().Be(QueueStatus.InAttendance);
    }

    [Fact]
    public void CallForAttendance_deve_falhar_quando_nao_esta_aguardando()
    {
        var entry = CreateEntry();
        entry.CallForAttendance();

        var result = entry.CallForAttendance();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(QueueErrors.TransicaoInvalida);
    }

    [Fact]
    public void MarkAsAttended_deve_concluir_fila_e_zerar_faltas_consecutivas()
    {
        var entry = CreateEntry();
        entry.RegisterAbsence();

        var result = entry.MarkAsAttended();

        result.IsSuccess.Should().BeTrue();
        entry.Status.Should().Be(QueueStatus.Completed);
        entry.ConsecutiveAbsences.Should().Be(0);
        entry.IsActive().Should().BeFalse();
    }

    [Fact]
    public void MarkAsAttended_deve_falhar_quando_fila_nao_esta_ativa()
    {
        var entry = CreateEntry();
        entry.MarkAsAttended();

        var result = entry.MarkAsAttended();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(QueueErrors.FilaNaoAtiva);
    }

    [Fact]
    public void RegisterAbsence_deve_marcar_absent_na_primeira_falta()
    {
        var entry = CreateEntry();

        var result = entry.RegisterAbsence();

        result.IsSuccess.Should().BeTrue();
        entry.Status.Should().Be(QueueStatus.Absent);
        entry.ConsecutiveAbsences.Should().Be(1);
    }

    [Fact]
    public void ReclassifyPriority_deve_falhar_sem_justificativa()
    {
        var entry = CreateEntry();

        var result = entry.ReclassifyPriority(QueuePriority.Urgent, justification: null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(QueueErrors.JustificativaObrigatoria);
    }

    [Fact]
    public void ReclassifyPriority_deve_falhar_com_justificativa_muito_curta()
    {
        var entry = CreateEntry();

        var result = entry.ReclassifyPriority(QueuePriority.Urgent, "curta");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(QueueErrors.JustificativaMuitoCurta);
    }

    [Fact]
    public void ReclassifyPriority_deve_ter_sucesso_com_justificativa_valida()
    {
        var entry = CreateEntry(QueuePriority.WaitingList);

        var result = entry.ReclassifyPriority(QueuePriority.Urgent, "Piora clínica relatada pela família");

        result.IsSuccess.Should().BeTrue();
        entry.Priority.Should().Be(QueuePriority.Urgent);
    }

    [Fact]
    public void WaitingTime_deve_calcular_diferenca_desde_a_entrada()
    {
        var enteredAt = new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc);
        var entry = QueueEntry.Create(Guid.NewGuid(), Guid.NewGuid(), "Psicologia", QueuePriority.Urgent, enteredAt).Value;

        var waitingTime = entry.WaitingTime(enteredAt.AddDays(3));

        waitingTime.Should().Be(TimeSpan.FromDays(3));
    }
}
