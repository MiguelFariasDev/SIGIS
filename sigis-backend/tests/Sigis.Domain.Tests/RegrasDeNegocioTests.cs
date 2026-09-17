using FluentAssertions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.ValueObjects;

namespace Sigis.Domain.Tests;

/// <summary>
/// Testes que documentam explicitamente as regras de negócio críticas
/// (RN01–RN07) do núcleo de domínio do SIGIS, uma por método de teste,
/// para servirem como referência viva das decisões de <c>claude.md</c>.
/// </summary>
public class RegrasDeNegocioTests
{
    private static readonly PersonName SampleName = PersonName.Create("Maria da Silva").Value;
    private static readonly EmailAddress SampleEmail = EmailAddress.Create("maria.silva@sigis.gov.br").Value;
    private const string SamplePasswordHash = "$2a$12$fakehashforunittestsonly000000000000000000000000000";

    [Fact(DisplayName = "RN01: fila ativa é única por pessoa e unidade")]
    public void RN01_QueueEntry_deve_ser_considerada_ativa_apenas_em_waiting_ou_in_attendance()
    {
        // A unicidade em si (impedir duas filas ativas para a mesma pessoa na
        // mesma unidade) é garantida pelo índice único parcial do banco de
        // dados e verificada pela camada de aplicação (fora do escopo deste
        // prompt, que cobre apenas Sigis.Domain). O contrato de domínio que
        // sustenta essa regra é QueueEntry.IsActive(): só uma entrada "ativa"
        // pode conflitar com outra ativa para o mesmo par (pessoa, unidade).
        var entry = QueueEntry.Create(Guid.NewGuid(), Guid.NewGuid(), "Psicologia", QueuePriority.Urgent, DateTime.UtcNow).Value;

        entry.IsActive().Should().BeTrue();

        entry.MarkAsAttended();

        entry.IsActive().Should().BeFalse();
    }

    [Fact(DisplayName = "RN02: alerta de duplicidade só é criado com score >= 0,75")]
    public void RN02_DuplicateAlert_deve_exigir_score_maior_ou_igual_ao_limiar()
    {
        var abaixoDoLimiar = DuplicateAlert.Create(Guid.NewGuid(), Guid.NewGuid(), 0.74, DateTime.UtcNow);
        var noLimiar = DuplicateAlert.Create(Guid.NewGuid(), Guid.NewGuid(), 0.75, DateTime.UtcNow);

        abaixoDoLimiar.IsFailure.Should().BeTrue();
        abaixoDoLimiar.Error.Should().Be(DuplicateErrors.ScoreAbaixoLimiar);

        noLimiar.IsSuccess.Should().BeTrue();
    }

    [Fact(DisplayName = "RN03: reclassificação de prioridade exige justificativa com ao menos 10 caracteres")]
    public void RN03_ReclassifyPriority_deve_exigir_justificativa_minima()
    {
        var entry = QueueEntry.Create(Guid.NewGuid(), Guid.NewGuid(), "Psicologia", QueuePriority.WaitingList, DateTime.UtcNow).Value;

        var semJustificativa = entry.ReclassifyPriority(QueuePriority.Urgent, null);
        var justificativaCurta = entry.ReclassifyPriority(QueuePriority.Urgent, "123456789"); // 9 caracteres
        var justificativaValida = entry.ReclassifyPriority(QueuePriority.Urgent, "1234567890"); // 10 caracteres

        semJustificativa.Error.Should().Be(QueueErrors.JustificativaObrigatoria);
        justificativaCurta.Error.Should().Be(QueueErrors.JustificativaMuitoCurta);
        justificativaValida.IsSuccess.Should().BeTrue();
    }

    [Fact(DisplayName = "RN04: 2 faltas consecutivas enviam a fila para busca ativa")]
    public void RN04_QueueEntry_deve_ir_para_busca_ativa_apos_duas_faltas_consecutivas()
    {
        var entry = QueueEntry.Create(Guid.NewGuid(), Guid.NewGuid(), "Psicologia", QueuePriority.ShortTerm, DateTime.UtcNow).Value;

        entry.RegisterAbsence(); // 1ª falta
        entry.Status.Should().Be(QueueStatus.Absent);

        entry.RegisterAbsence(); // 2ª falta consecutiva
        entry.Status.Should().Be(QueueStatus.ActiveSearch);
    }

    [Fact(DisplayName = "RN04: comparecimento intermediário reinicia a contagem de faltas consecutivas")]
    public void RN04_MarkAsAttended_deve_zerar_contagem_de_faltas_consecutivas()
    {
        var entry = QueueEntry.Create(Guid.NewGuid(), Guid.NewGuid(), "Psicologia", QueuePriority.ShortTerm, DateTime.UtcNow).Value;

        entry.RegisterAbsence();
        entry.MarkAsAttended();

        entry.ConsecutiveAbsences.Should().Be(0);
    }

    [Fact(DisplayName = "RN05: encaminhamento só pode ser concluído a partir de Accepted")]
    public void RN05_Referral_so_deve_concluir_apos_ser_aceito()
    {
        var referral = Referral.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "Necessita avaliação especializada",
            QueuePriority.ShortTerm, DateTime.UtcNow).Value;

        var completouSemAceitar = referral.RegisterFirstAttendance();
        completouSemAceitar.IsFailure.Should().BeTrue();
        completouSemAceitar.Error.Should().Be(ReferralErrors.NaoAceito);

        referral.Accept();
        var completouAposAceitar = referral.RegisterFirstAttendance();

        completouAposAceitar.IsSuccess.Should().BeTrue();
        referral.Status.Should().Be(ReferralStatus.Completed);
    }

    [Theory(DisplayName = "RN06: apenas Coordinator e Auditor enxergam a rede completa")]
    [InlineData(RbacRole.Professional, false)]
    [InlineData(RbacRole.Coordinator, true)]
    [InlineData(RbacRole.Auditor, true)]
    public void RN06_CanSeeFullNetwork_deve_ser_restrito_a_coordinator_e_auditor(RbacRole role, bool expected)
    {
        var professional = Professional.Create(SampleName, "Psicólogo", Guid.NewGuid(), role, SampleEmail, SamplePasswordHash).Value;

        professional.CanSeeFullNetwork().Should().Be(expected);
    }

    [Theory(DisplayName = "RN07: apenas Auditor pode consultar os logs de acesso")]
    [InlineData(RbacRole.Professional, false)]
    [InlineData(RbacRole.Coordinator, false)]
    [InlineData(RbacRole.Auditor, true)]
    public void RN07_CanSeeLogs_deve_ser_restrito_a_auditor(RbacRole role, bool expected)
    {
        var professional = Professional.Create(SampleName, "Psicólogo", Guid.NewGuid(), role, SampleEmail, SamplePasswordHash).Value;

        professional.CanSeeLogs().Should().Be(expected);
    }
}
