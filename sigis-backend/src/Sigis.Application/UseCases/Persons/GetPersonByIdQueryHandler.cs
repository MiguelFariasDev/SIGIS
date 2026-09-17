using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Persons;

/// <summary>Processa a consulta dos dados completos de uma pessoa pelo identificador.</summary>
public sealed class GetPersonByIdQueryHandler : IRequestHandler<GetPersonByIdQuery, Result<PersonDetailResponse>>
{
    private readonly IPersonRepository _personRepository;

    /// <summary>Cria o handler de consulta de pessoa por identificador.</summary>
    public GetPersonByIdQueryHandler(IPersonRepository personRepository) => _personRepository = personRepository;

    /// <inheritdoc />
    public async Task<Result<PersonDetailResponse>> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<PersonDetailResponse>.Failure(PersonErrors.PessoaNaoEncontrada);

        return Result<PersonDetailResponse>.Success(PersonMapper.ToDetailResponse(person));
    }
}
