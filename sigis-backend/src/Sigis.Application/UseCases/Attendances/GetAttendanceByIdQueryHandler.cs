using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Attendances;

/// <summary>Processa a consulta de um atendimento pelo identificador.</summary>
public sealed class GetAttendanceByIdQueryHandler : IRequestHandler<GetAttendanceByIdQuery, Result<AttendanceResponse>>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;
    private readonly IProfessionalRepository _professionalRepository;

    /// <summary>Cria o handler de consulta de atendimento por id.</summary>
    public GetAttendanceByIdQueryHandler(
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
    public async Task<Result<AttendanceResponse>> Handle(GetAttendanceByIdQuery request, CancellationToken cancellationToken)
    {
        var attendance = await _attendanceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (attendance is null)
            return Result<AttendanceResponse>.Failure(CommonErrors.NotFound);

        return Result<AttendanceResponse>.Success(await AttendanceResponseMapper.MapAsync(
            attendance, _personRepository, _serviceUnitRepository, _professionalRepository, cancellationToken));
    }
}
