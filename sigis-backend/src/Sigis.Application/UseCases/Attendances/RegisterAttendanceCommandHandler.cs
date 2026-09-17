using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Entities;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Attendances;

/// <summary>
/// Processa o registro de um novo atendimento, calculando automaticamente o
/// número sequencial da sessão para a pessoa e o tipo de sessão informados.
/// </summary>
public sealed class RegisterAttendanceCommandHandler : IRequestHandler<RegisterAttendanceCommand, Result<AttendanceResponse>>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IServiceUnitRepository _serviceUnitRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de registro de atendimento.</summary>
    public RegisterAttendanceCommandHandler(
        IAttendanceRepository attendanceRepository,
        IPersonRepository personRepository,
        IServiceUnitRepository serviceUnitRepository,
        IDateTimeProvider dateTimeProvider,
        IUnitOfWork unitOfWork)
    {
        _attendanceRepository = attendanceRepository;
        _personRepository = personRepository;
        _serviceUnitRepository = serviceUnitRepository;
        _dateTimeProvider = dateTimeProvider;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<AttendanceResponse>> Handle(RegisterAttendanceCommand request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<AttendanceResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        var unit = await _serviceUnitRepository.GetByIdAsync(request.UnitId, cancellationToken);
        if (unit is null)
            return Result<AttendanceResponse>.Failure(CommonErrors.NotFound);

        if (!Enum.TryParse<SessionType>(request.SessionType, ignoreCase: true, out var sessionType))
            return Result<AttendanceResponse>.Failure(AttendanceErrors.TipoSessaoObrigatorio);

        var history = await _attendanceRepository.GetByPersonAsync(request.PersonId, cancellationToken);
        var sessionNumber = history.Count(a => a.SessionType == sessionType) + 1;

        var createResult = Attendance.Create(
            request.PersonId,
            request.UnitId,
            request.ProfessionalId,
            request.DateTime,
            sessionType,
            sessionNumber,
            _dateTimeProvider.UtcNow,
            request.FormData,
            request.TriagedByProfessionalId,
            request.MainComplaint);

        if (createResult.IsFailure)
            return Result<AttendanceResponse>.Failure(createResult.Error);

        var attendance = createResult.Value;
        await _attendanceRepository.AddAsync(attendance, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AttendanceResponse>.Success(new AttendanceResponse(
            attendance.Id, attendance.PersonId, attendance.UnitId, attendance.ProfessionalId, attendance.DateTime,
            attendance.SessionType.ToString(), attendance.SessionNumber, attendance.Status.ToString(),
            attendance.FormData, attendance.TriagedByProfessionalId, attendance.MainComplaint, attendance.CreatedAt));
    }
}
