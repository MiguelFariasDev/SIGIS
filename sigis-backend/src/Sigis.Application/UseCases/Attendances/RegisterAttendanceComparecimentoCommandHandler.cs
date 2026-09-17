using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Attendances;

/// <summary>
/// Processa o registro de comparecimento (ou falta) em um atendimento
/// agendado, opcionalmente registrando a triagem quando uma queixa principal
/// é informada.
/// </summary>
public sealed class RegisterAttendanceComparecimentoCommandHandler
    : IRequestHandler<RegisterAttendanceComparecimentoCommand, Result<AttendanceResponse>>
{
    private readonly IAttendanceRepository _attendanceRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de registro de comparecimento em atendimento.</summary>
    public RegisterAttendanceComparecimentoCommandHandler(
        IAttendanceRepository attendanceRepository, ICurrentUserService currentUserService, IUnitOfWork unitOfWork)
    {
        _attendanceRepository = attendanceRepository;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result<AttendanceResponse>> Handle(
        RegisterAttendanceComparecimentoCommand request, CancellationToken cancellationToken)
    {
        var attendance = await _attendanceRepository.GetByIdAsync(request.AttendanceId, cancellationToken);
        if (attendance is null)
            return Result<AttendanceResponse>.Failure(CommonErrors.NotFound);

        var comparecimento = request.Comparecimento?.Trim().ToUpperInvariant();
        if (comparecimento is not ("COMPARECEU" or "FALTOU"))
            return Result<AttendanceResponse>.Failure(new Error(
                "ATTENDANCE_VALIDATION", "Comparecimento deve ser \"COMPARECEU\" ou \"FALTOU\"."));

        if (!string.IsNullOrWhiteSpace(request.MainComplaint) && _currentUserService.ProfessionalId.HasValue)
        {
            var triageResult = attendance.RegisterTriage(_currentUserService.ProfessionalId.Value, request.MainComplaint);
            if (triageResult.IsFailure)
                return Result<AttendanceResponse>.Failure(triageResult.Error);
        }

        var registrationResult = comparecimento == "COMPARECEU" ? attendance.MarkAsAttended() : attendance.MarkAsAbsent();
        if (registrationResult.IsFailure)
            return Result<AttendanceResponse>.Failure(registrationResult.Error);

        await _attendanceRepository.UpdateAsync(attendance, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AttendanceResponse>.Success(new AttendanceResponse(
            attendance.Id, attendance.PersonId, attendance.UnitId, attendance.ProfessionalId, attendance.DateTime,
            attendance.SessionType.ToString(), attendance.SessionNumber, attendance.Status.ToString(),
            attendance.FormData, attendance.TriagedByProfessionalId, attendance.MainComplaint, attendance.CreatedAt));
    }
}
