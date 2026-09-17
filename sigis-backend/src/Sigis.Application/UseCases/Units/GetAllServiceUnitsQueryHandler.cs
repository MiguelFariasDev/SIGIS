using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Units;

/// <summary>Processa a listagem de todas as unidades de serviço da rede.</summary>
public sealed class GetAllServiceUnitsQueryHandler
    : IRequestHandler<GetAllServiceUnitsQuery, Result<IReadOnlyList<ServiceUnitResponse>>>
{
    private readonly IServiceUnitRepository _serviceUnitRepository;

    /// <summary>Cria o handler de listagem de unidades.</summary>
    public GetAllServiceUnitsQueryHandler(IServiceUnitRepository serviceUnitRepository)
    {
        _serviceUnitRepository = serviceUnitRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<ServiceUnitResponse>>> Handle(
        GetAllServiceUnitsQuery request, CancellationToken cancellationToken)
    {
        var units = await _serviceUnitRepository.GetAllAsync(cancellationToken);

        IReadOnlyList<ServiceUnitResponse> response = units
            .Select(u => new ServiceUnitResponse(u.Id, u.Name, u.Acronym, u.Secretariat.ToString()))
            .ToList();

        return Result<IReadOnlyList<ServiceUnitResponse>>.Success(response);
    }
}
