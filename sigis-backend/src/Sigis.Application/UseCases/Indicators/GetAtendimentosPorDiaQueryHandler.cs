using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Indicators;

/// <summary>Processa a consulta de atendimentos por dia no período informado.</summary>
public sealed class GetAtendimentosPorDiaQueryHandler
    : IRequestHandler<GetAtendimentosPorDiaQuery, Result<IReadOnlyList<AtendimentosPorDiaResponse>>>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>Cria o handler de consulta de atendimentos por dia.</summary>
    public GetAtendimentosPorDiaQueryHandler(
        IAttendanceRepository attendanceRepository, IDateTimeProvider dateTimeProvider)
    {
        _attendanceRepository = attendanceRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<AtendimentosPorDiaResponse>>> Handle(
        GetAtendimentosPorDiaQuery request, CancellationToken cancellationToken)
    {
        var dias = request.Periodo switch
        {
            "HOJE" => 0,
            "30D" => 30,
            _ => 7,
        };

        var to = _dateTimeProvider.UtcNow;
        var from = to.Date.AddDays(-dias);

        var attendances = await _attendanceRepository.SearchAsync(null, from, to, cancellationToken);

        var response = attendances
            .GroupBy(a => DateOnly.FromDateTime(a.DateTime))
            .Select(g => new AtendimentosPorDiaResponse(g.Key, g.Count()))
            .OrderBy(r => r.Data)
            .ToList();

        return Result<IReadOnlyList<AtendimentosPorDiaResponse>>.Success(response);
    }
}
