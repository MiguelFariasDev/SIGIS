using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Development;

/// <summary>Processa a consulta dos marcos de desenvolvimento de uma pessoa.</summary>
public sealed class GetDevelopmentMilestonesQueryHandler
    : IRequestHandler<GetDevelopmentMilestonesQuery, Result<DevelopmentMilestonesResponse>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IDevelopmentMilestonesRepository _developmentMilestonesRepository;

    /// <summary>Cria o handler de consulta de marcos de desenvolvimento.</summary>
    public GetDevelopmentMilestonesQueryHandler(
        IPersonRepository personRepository, IDevelopmentMilestonesRepository developmentMilestonesRepository)
    {
        _personRepository = personRepository;
        _developmentMilestonesRepository = developmentMilestonesRepository;
    }

    /// <inheritdoc />
    public async Task<Result<DevelopmentMilestonesResponse>> Handle(
        GetDevelopmentMilestonesQuery request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<DevelopmentMilestonesResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        var milestones = await _developmentMilestonesRepository.GetByPersonAsync(request.PersonId, cancellationToken);
        if (milestones is null)
            return Result<DevelopmentMilestonesResponse>.Failure(CommonErrors.NotFound);

        return Result<DevelopmentMilestonesResponse>.Success(DevelopmentMilestonesMapper.ToResponse(milestones));
    }
}
