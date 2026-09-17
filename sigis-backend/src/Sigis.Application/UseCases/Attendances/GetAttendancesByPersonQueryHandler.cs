using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Attendances;

/// <summary>Processa a consulta do histórico de atendimentos de uma pessoa.</summary>
public sealed class GetAttendancesByPersonQueryHandler
    : IRequestHandler<GetAttendancesByPersonQuery, Result<List<AttendanceResponse>>>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;
    private readonly IProfessionalRepository _professionalRepository;

    /// <summary>Cria o handler de consulta de atendimentos por pessoa.</summary>
    public GetAttendancesByPersonQueryHandler(
        IAttendanceRepository attendanceRepository,
        IPersonRepository personRepository,
        IServiceUnitRepository serviceUnitRepository,
        IProfessionalRepository professionalRepository)
    {
        _attendanceRepository = attendanceRepository;
        _personRepository = personRepository;
        _serviceUnitRepository = serviceUnitRepository;
        _professionalRepository = professionalRepository;
    }

    /// <inheritdoc />
    public async Task<Result<List<AttendanceResponse>>> Handle(
        GetAttendancesByPersonQuery request, CancellationToken cancellationToken)
    {
        var attendances = await _attendanceRepository.GetByPersonAsync(request.PersonId, cancellationToken);

        var responses = new List<AttendanceResponse>(attendances.Count);
        foreach (var attendance in attendances)
            responses.Add(await AttendanceResponseMapper.MapAsync(
                attendance, _personRepository, _serviceUnitRepository, _professionalRepository, cancellationToken));

        return Result<List<AttendanceResponse>>.Success(responses);
    }
}
