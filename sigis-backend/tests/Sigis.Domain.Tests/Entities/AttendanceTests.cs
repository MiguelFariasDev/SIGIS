using FluentAssertions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.Events;

namespace Sigis.Domain.Tests.Entities;

public class AttendanceTests
{
    private static Attendance CreateAttendance() => Attendance.Create(
        Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow,
        SessionType.PsychologicalAnamnesis, sessionNumber: 1, createdAt: DateTime.UtcNow).Value;

    [Fact]
    public void Create_deve_ter_sucesso_com_status_inicial_scheduled()
    {
        var attendance = CreateAttendance();

        attendance.Status.Should().Be(AttendanceStatus.Scheduled);
    }

    [Fact]
    public void MarkAsAttended_deve_ter_sucesso_e_emitir_AttendanceRegisteredEvent()
    {
        var attendance = CreateAttendance();

        var result = attendance.MarkAsAttended();

        result.IsSuccess.Should().BeTrue();
        attendance.Status.Should().Be(AttendanceStatus.Attended);
        attendance.Events.Should().ContainSingle().Which.Should().BeOfType<AttendanceRegisteredEvent>();
    }

    [Fact]
    public void MarkAsAttended_deve_falhar_quando_ja_registrado()
    {
        var attendance = CreateAttendance();
        attendance.MarkAsAttended();

        var result = attendance.MarkAsAttended();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AttendanceErrors.ComparecimentoJaRegistrado);
    }

    [Fact]
    public void MarkAsAbsent_deve_ter_sucesso_e_emitir_AbsenceRegisteredEvent()
    {
        var attendance = CreateAttendance();

        var result = attendance.MarkAsAbsent();

        result.IsSuccess.Should().BeTrue();
        attendance.Status.Should().Be(AttendanceStatus.Absent);
        attendance.Events.Should().ContainSingle().Which.Should().BeOfType<AbsenceRegisteredEvent>();
    }

    [Fact]
    public void MarkAsAbsent_deve_falhar_quando_ja_registrado_comparecimento()
    {
        var attendance = CreateAttendance();
        attendance.MarkAsAttended();

        var result = attendance.MarkAsAbsent();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AttendanceErrors.ComparecimentoJaRegistrado);
    }

    [Fact]
    public void UpdateFormData_deve_ter_sucesso_enquanto_agendado()
    {
        var attendance = CreateAttendance();

        var result = attendance.UpdateFormData("{\"nota\":\"ok\"}");

        result.IsSuccess.Should().BeTrue();
        attendance.FormData.Should().Be("{\"nota\":\"ok\"}");
    }

    [Fact]
    public void UpdateFormData_deve_falhar_apos_comparecimento_registrado()
    {
        var attendance = CreateAttendance();
        attendance.MarkAsAttended();

        var result = attendance.UpdateFormData("{\"nota\":\"ok\"}");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AttendanceErrors.NaoPodeAlterarAposComparecimento);
    }

    [Fact]
    public void UpdateFormData_deve_falhar_com_json_vazio()
    {
        var attendance = CreateAttendance();

        var result = attendance.UpdateFormData(string.Empty);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(AttendanceErrors.DadosFormularioInvalidos);
    }
}
