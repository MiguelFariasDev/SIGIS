using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Indicators;

/// <summary>
/// Processa <see cref="GetIndicatorsPanelQuery"/>, agregando em memória os
/// registros de fila, atendimentos e encaminhamentos que atendem ao filtro —
/// aceitável na escala de dados do hackathon; não introduz agregação no
/// banco por não haver necessidade de escala além disso no momento.
/// </summary>
public sealed class GetIndicatorsPanelQueryHandler : IRequestHandler<GetIndicatorsPanelQuery, Result<IndicatorsResponse>>
{
    private readonly IQueueEntryRepository _queueEntryRepository;
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IReferralRepository _referralRepository;

    /// <summary>Cria o handler de <see cref="GetIndicatorsPanelQuery"/>.</summary>
    /// <param name="queueEntryRepository">Repositório de entradas de fila.</param>
    /// <param name="attendanceRepository">Repositório de atendimentos.</param>
    /// <param name="referralRepository">Repositório de encaminhamentos.</param>
    public GetIndicatorsPanelQueryHandler(
        IQueueEntryRepository queueEntryRepository,
        IAttendanceRepository attendanceRepository,
        IReferralRepository referralRepository)
    {
        _queueEntryRepository = queueEntryRepository;
        _attendanceRepository = attendanceRepository;
        _referralRepository = referralRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IndicatorsResponse>> Handle(
        GetIndicatorsPanelQuery request, CancellationToken cancellationToken)
    {
        var queueEntries = await _queueEntryRepository.SearchAsync(
            request.UnitId, request.From, request.To, cancellationToken);
        var attendances = await _attendanceRepository.SearchAsync(
            request.UnitId, request.From, request.To, cancellationToken);
        var referrals = await _referralRepository.SearchAsync(
            request.UnitId, request.From, request.To, cancellationToken);

        var filaPorStatus = queueEntries
            .GroupBy(q => q.Status)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        var filaPorPrioridade = queueEntries
            .GroupBy(q => q.Priority)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        var concluidas = queueEntries.Where(q => q.Status == QueueStatus.Completed).ToList();
        double? tempoMedioEsperaMinutos = concluidas.Count > 0
            ? concluidas.Average(q => (q.UpdatedAt - q.EnteredAt).TotalMinutes)
            : null;

        var atendimentosPorStatus = attendances
            .GroupBy(a => a.Status)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        var encaminhamentosPorStatus = referrals
            .GroupBy(r => r.Status)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        var response = new IndicatorsResponse(
            TotalNaFilaAtiva: queueEntries.Count(q => q.IsActive()),
            FilaPorStatus: filaPorStatus,
            FilaPorPrioridade: filaPorPrioridade,
            TempoMedioEsperaMinutos: tempoMedioEsperaMinutos,
            TotalAtendimentos: attendances.Count,
            AtendimentosPorStatus: atendimentosPorStatus,
            TotalEncaminhamentos: referrals.Count,
            EncaminhamentosPorStatus: encaminhamentosPorStatus);

        return Result<IndicatorsResponse>.Success(response);
    }
}
