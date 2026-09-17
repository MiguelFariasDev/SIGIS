using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.SchoolHistories;

/// <summary>Processa a consulta do histórico escolar completo de uma pessoa.</summary>
public sealed class GetSchoolHistoryQueryHandler
    : IRequestHandler<GetSchoolHistoryQuery, Result<IReadOnlyList<SchoolHistoryResponse>>>
{
    private readonly IPersonRepository _personRepository;
    private readonly ISchoolHistoryRepository _schoolHistoryRepository;

    /// <summary>Cria o handler de consulta de histórico escolar.</summary>
    public GetSchoolHistoryQueryHandler(IPersonRepository personRepository, ISchoolHistoryRepository schoolHistoryRepository)
    {
        _personRepository = personRepository;
        _schoolHistoryRepository = schoolHistoryRepository;
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<SchoolHistoryResponse>>> Handle(
        GetSchoolHistoryQuery request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.PersonId, cancellationToken);
        if (person is null)
            return Result<IReadOnlyList<SchoolHistoryResponse>>.Failure(PersonErrors.PessoaNaoEncontrada);

        var records = await _schoolHistoryRepository.GetByPersonAsync(request.PersonId, cancellationToken);

        var response = records
            .Select(r => new SchoolHistoryResponse(
                r.Id, r.PersonId, r.SchoolName, r.Grade, r.Shift, r.ClassGroup, r.SchoolYear,
                r.StartDate, r.EndDate, r.Status, r.Notes, r.CreatedAt, r.CreatedByProfessionalId))
            .ToList();

        return Result<IReadOnlyList<SchoolHistoryResponse>>.Success(response);
    }
}
