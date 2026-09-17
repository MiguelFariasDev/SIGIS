using FluentAssertions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.Events;

namespace Sigis.Domain.Tests.Entities;

public class ReferralTests
{
    private static readonly Guid OriginUnitId = Guid.NewGuid();
    private static readonly Guid DestinationUnitId = Guid.NewGuid();

    private static Referral CreateReferral() => Referral.Create(
        Guid.NewGuid(), OriginUnitId, DestinationUnitId, "Necessita avaliação especializada",
        QueuePriority.ShortTerm, DateTime.UtcNow).Value;

    [Fact]
    public void Create_deve_ter_sucesso_com_status_inicial_pending()
    {
        var referral = CreateReferral();

        referral.Status.Should().Be(ReferralStatus.Pending);
    }

    [Fact]
    public void Create_deve_emitir_ReferralCreatedEvent()
    {
        var referral = CreateReferral();

        referral.Events.Should().ContainSingle().Which.Should().BeOfType<ReferralCreatedEvent>();
    }

    [Fact]
    public void Create_deve_falhar_quando_origem_igual_destino()
    {
        var sameUnit = Guid.NewGuid();

        var result = Referral.Create(
            Guid.NewGuid(), sameUnit, sameUnit, "Necessita avaliação especializada",
            QueuePriority.ShortTerm, DateTime.UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReferralErrors.OrigemIgualDestino);
    }

    [Fact]
    public void Create_deve_falhar_sem_motivo()
    {
        var result = Referral.Create(
            Guid.NewGuid(), OriginUnitId, DestinationUnitId, null, QueuePriority.ShortTerm, DateTime.UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReferralErrors.MotivoObrigatorio);
    }

    [Fact]
    public void Create_deve_falhar_com_motivo_muito_curto()
    {
        var result = Referral.Create(
            Guid.NewGuid(), OriginUnitId, DestinationUnitId, "curto", QueuePriority.ShortTerm, DateTime.UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReferralErrors.MotivoMuitoCurto);
    }

    [Fact]
    public void Accept_deve_transicionar_de_pending_para_accepted()
    {
        var referral = CreateReferral();

        var result = referral.Accept();

        result.IsSuccess.Should().BeTrue();
        referral.Status.Should().Be(ReferralStatus.Accepted);
    }

    [Fact]
    public void Accept_deve_falhar_quando_nao_esta_pending()
    {
        var referral = CreateReferral();
        referral.Accept();

        var result = referral.Accept();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReferralErrors.TransicaoInvalida);
    }

    [Fact]
    public void RegisterFirstAttendance_deve_falhar_quando_nao_aceito_RN05()
    {
        var referral = CreateReferral();

        var result = referral.RegisterFirstAttendance();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReferralErrors.NaoAceito);
    }

    [Fact]
    public void RegisterFirstAttendance_deve_concluir_apos_aceito_e_emitir_evento()
    {
        var referral = CreateReferral();
        referral.Accept();

        var result = referral.RegisterFirstAttendance();

        result.IsSuccess.Should().BeTrue();
        referral.Status.Should().Be(ReferralStatus.Completed);
        referral.Events.Should().Contain(e => e is ReferralCompletedEvent);
    }

    [Fact]
    public void Refuse_deve_transicionar_para_refused_com_motivo()
    {
        var referral = CreateReferral();

        var result = referral.Refuse("Unidade sem vaga na especialidade");

        result.IsSuccess.Should().BeTrue();
        referral.Status.Should().Be(ReferralStatus.Refused);
    }

    [Fact]
    public void Refuse_deve_falhar_sem_motivo()
    {
        var referral = CreateReferral();

        var result = referral.Refuse(null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ReferralErrors.MotivoObrigatorio);
    }
}
