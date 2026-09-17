using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Duplicates;

/// <summary>Processa a consulta de um alerta de duplicidade pelo identificador.</summary>
public sealed class GetDuplicateByIdQueryHandler : IRequestHandler<GetDuplicateByIdQuery, Result<DuplicateAlertResponse>>
{
    private readonly IDuplicateAlertRepository _duplicateAlertRepository;
    private readonly IPersonRepository _personRepository;

    /// <summary>Cria o handler de consulta de um alerta de duplicidade por id.</summary>
    public GetDuplicateByIdQueryHandler(IDuplicateAlertRepository duplicateAlertRepository, IPersonRepository personRepository)
    {
        _duplicateAlertRepository = duplicateAlertRepository;
        _personRepository = personRepository;
    }

    /// <inheritdoc />
    public async Task<Result<DuplicateAlertResponse>> Handle(GetDuplicateByIdQuery request, CancellationToken cancellationToken)
    {
        var alert = await _duplicateAlertRepository.GetByIdAsync(request.Id, cancellationToken);
        if (alert is null)
            return Result<DuplicateAlertResponse>.Failure(CommonErrors.NotFound);

        return Result<DuplicateAlertResponse>.Success(
            await DuplicateAlertResponseMapper.MapAsync(alert, _personRepository, cancellationToken));
    }
}
