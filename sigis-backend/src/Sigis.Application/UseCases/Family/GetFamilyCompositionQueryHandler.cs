using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Family;

/// <summary>Processa a consulta da composição familiar de uma pessoa.</summary>
public sealed class GetFamilyCompositionQueryHandler : IRequestHandler<GetFamilyCompositionQuery, Result<FamilyCompositionResponse>>
{
    private readonly IPersonRepository _personRepository;
    private readonly IFamilyCompositionRepository _familyCompositionRepository;

    /// <summary>Cria o handler de consulta de composição familiar.</summary>
    public GetFamilyCompositionQueryHandler(
        IPersonRepository personRepository, IFamilyCompositionRepository familyCompositionRepository)
    {
        _personRepository = personRepository;
        _familyCompositionRepository = familyCompositionRepository;
    }

    /// <inheritdoc />
    public async Task<Result<FamilyCompositionResponse>> Handle(GetFamilyCompositionQuery request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<FamilyCompositionResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        var composition = await _familyCompositionRepository.GetByPersonAsync(request.PersonId, cancellationToken);
        if (composition is null)
            return Result<FamilyCompositionResponse>.Failure(CommonErrors.NotFound);

        return Result<FamilyCompositionResponse>.Success(FamilyCompositionMapper.ToResponse(composition));
    }
}
