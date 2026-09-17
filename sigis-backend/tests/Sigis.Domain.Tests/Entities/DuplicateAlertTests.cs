using FluentAssertions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.Events;

namespace Sigis.Domain.Tests.Entities;

public class DuplicateAlertTests
{
    private static readonly Guid PersonId1 = Guid.NewGuid();
    private static readonly Guid PersonId2 = Guid.NewGuid();

    private static DuplicateAlert CreateAlert(double score = 0.9) =>
        DuplicateAlert.Create(PersonId1, PersonId2, score, DateTime.UtcNow).Value;

    [Fact]
    public void Create_deve_ter_sucesso_com_status_inicial_pending()
    {
        var alert = CreateAlert();

        alert.Status.Should().Be(DuplicateAlertStatus.Pending);
        alert.IsPending().Should().BeTrue();
    }

    [Fact]
    public void Create_deve_emitir_DuplicateDetectedEvent()
    {
        var alert = CreateAlert(0.9);

        alert.Events.Should().ContainSingle()
            .Which.Should().BeOfType<DuplicateDetectedEvent>()
            .Which.SimilarityScore.Should().Be(0.9);
    }

    [Fact]
    public void Create_deve_falhar_para_mesma_pessoa()
    {
        var result = DuplicateAlert.Create(PersonId1, PersonId1, 0.9, DateTime.UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DuplicateErrors.MesmaPessoa);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void Create_deve_falhar_com_score_fora_do_intervalo(double score)
    {
        var result = DuplicateAlert.Create(PersonId1, PersonId2, score, DateTime.UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DuplicateErrors.ScoreInvalido);
    }

    [Fact]
    public void Create_deve_falhar_com_score_abaixo_do_limiar_RN02()
    {
        var result = DuplicateAlert.Create(PersonId1, PersonId2, 0.5, DateTime.UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DuplicateErrors.ScoreAbaixoLimiar);
    }

    [Fact]
    public void Create_deve_ter_sucesso_exatamente_no_limiar_RN02()
    {
        var result = DuplicateAlert.Create(PersonId1, PersonId2, 0.75, DateTime.UtcNow);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void ConfirmMerge_deve_transicionar_para_merged()
    {
        var alert = CreateAlert();
        var professionalId = Guid.NewGuid();

        var result = alert.ConfirmMerge(professionalId, DateTime.UtcNow);

        result.IsSuccess.Should().BeTrue();
        alert.Status.Should().Be(DuplicateAlertStatus.Merged);
        alert.ResolvedByProfessionalId.Should().Be(professionalId);
    }

    [Fact]
    public void MarkAsFalsePositive_deve_transicionar_para_false_positive()
    {
        var alert = CreateAlert();
        var professionalId = Guid.NewGuid();

        var result = alert.MarkAsFalsePositive(professionalId, DateTime.UtcNow);

        result.IsSuccess.Should().BeTrue();
        alert.Status.Should().Be(DuplicateAlertStatus.FalsePositive);
    }

    [Fact]
    public void ConfirmMerge_deve_falhar_quando_ja_resolvido()
    {
        var alert = CreateAlert();
        alert.ConfirmMerge(Guid.NewGuid(), DateTime.UtcNow);

        var result = alert.ConfirmMerge(Guid.NewGuid(), DateTime.UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DuplicateErrors.AlertaJaResolvido);
    }

    [Fact]
    public void MarkAsFalsePositive_deve_falhar_quando_ja_resolvido()
    {
        var alert = CreateAlert();
        alert.MarkAsFalsePositive(Guid.NewGuid(), DateTime.UtcNow);

        var result = alert.MarkAsFalsePositive(Guid.NewGuid(), DateTime.UtcNow);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DuplicateErrors.AlertaJaResolvido);
    }
}
