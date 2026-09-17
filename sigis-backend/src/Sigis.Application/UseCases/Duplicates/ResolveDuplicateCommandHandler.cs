using MediatR;
using Sigis.Application.UseCases.Persons;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Duplicates;

/// <summary>
/// Processa a resolução genérica de um alerta de duplicidade, delegando
/// para <see cref="MergePersonsCommand"/> (ação <c>"MESCLAR"</c>, convenção:
/// o cadastro 2 é mesclado no cadastro 1) ou
/// <see cref="MarkDuplicateAsFalsePositiveCommand"/> (ação
/// <c>"FALSO_POSITIVO"</c>).
/// </summary>
public sealed class ResolveDuplicateCommandHandler : IRequestHandler<ResolveDuplicateCommand, Result<DuplicateAlertResponse>>
{
    private readonly IMediator _mediator;
    private readonly IDuplicateAlertRepository _duplicateAlertRepository;
    private readonly IPersonRepository _personRepository;

    /// <summary>Cria o handler de resolução de duplicidade.</summary>
    public ResolveDuplicateCommandHandler(
        IMediator mediator, IDuplicateAlertRepository duplicateAlertRepository, IPersonRepository personRepository)
    {
        _mediator = mediator;
        _duplicateAlertRepository = duplicateAlertRepository;
        _personRepository = personRepository;
    }

    /// <inheritdoc />
    public async Task<Result<DuplicateAlertResponse>> Handle(ResolveDuplicateCommand request, CancellationToken cancellationToken)
    {
        var alert = await _duplicateAlertRepository.GetByIdAsync(request.AlertId, cancellationToken);
        if (alert is null)
            return Result<DuplicateAlertResponse>.Failure(CommonErrors.NotFound);

        switch (request.Acao)
        {
            case "MESCLAR":
                var mergeResult = await _mediator.Send(
                    new MergePersonsCommand(alert.PersonId2, alert.PersonId1, alert.Id), cancellationToken);
                if (mergeResult.IsFailure)
                    return Result<DuplicateAlertResponse>.Failure(mergeResult.Error);
                break;

            case "FALSO_POSITIVO":
                var falsePositiveResult = await _mediator.Send(
                    new MarkDuplicateAsFalsePositiveCommand(alert.Id), cancellationToken);
                if (falsePositiveResult.IsFailure)
                    return Result<DuplicateAlertResponse>.Failure(falsePositiveResult.Error);
                break;

            default:
                return Result<DuplicateAlertResponse>.Failure(
                    new Error("DUPLICATE_VALIDATION", "Ação inválida — use MESCLAR ou FALSO_POSITIVO."));
        }

        var resolved = await _duplicateAlertRepository.GetByIdAsync(alert.Id, cancellationToken);
        return Result<DuplicateAlertResponse>.Success(
            await DuplicateAlertResponseMapper.MapAsync(resolved!, _personRepository, cancellationToken));
    }
}
