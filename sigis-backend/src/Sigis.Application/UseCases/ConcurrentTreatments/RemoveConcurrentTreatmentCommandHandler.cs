using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.ConcurrentTreatments;

/// <summary>Processa a remoção de um atendimento concomitante de uma pessoa.</summary>
public sealed class RemoveConcurrentTreatmentCommandHandler : IRequestHandler<RemoveConcurrentTreatmentCommand, Result>
{
    private readonly IConcurrentTreatmentRepository _concurrentTreatmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>Cria o handler de remoção de atendimento concomitante.</summary>
    public RemoveConcurrentTreatmentCommandHandler(
        IConcurrentTreatmentRepository concurrentTreatmentRepository, IUnitOfWork unitOfWork)
    {
        _concurrentTreatmentRepository = concurrentTreatmentRepository;
        _unitOfWork = unitOfWork;
    }

    /// <inheritdoc />
    public async Task<Result> Handle(RemoveConcurrentTreatmentCommand request, CancellationToken cancellationToken)
    {
        var treatment = await _concurrentTreatmentRepository.GetByIdAsync(request.TreatmentId, cancellationToken);
        if (treatment is null || treatment.PersonId != request.PersonId)
            return Result.Failure(CommonErrors.NotFound);

        await _concurrentTreatmentRepository.RemoveAsync(treatment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
