using System.Globalization;
using System.Text;
using MediatR;
using Sigis.Domain.Abstractions;
using Sigis.Domain.Interfaces;

namespace Sigis.Application.UseCases.Audit;

/// <summary>
/// Processa a exportação em CSV dos registros de auditoria de acesso (RF13)
/// — mesmos filtros de <see cref="GetAccessLogsQuery"/>. Não loga o conteúdo
/// do CSV (dados pessoais).
/// </summary>
public sealed class ExportAccessLogsCsvQueryHandler : IRequestHandler<ExportAccessLogsCsvQuery, Result<string>>
{
    private readonly IAccessLogRepository _accessLogRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IProfessionalRepository _professionalRepository;

    /// <summary>Cria o handler de exportação de auditoria em CSV.</summary>
    public ExportAccessLogsCsvQueryHandler(
        IAccessLogRepository accessLogRepository,
        IPersonRepository personRepository,
        IProfessionalRepository professionalRepository)
    {
        _accessLogRepository = accessLogRepository;
        _personRepository = personRepository;
        _professionalRepository = professionalRepository;
    }

    /// <inheritdoc />
    public async Task<Result<string>> Handle(ExportAccessLogsCsvQuery request, CancellationToken cancellationToken)
    {
        var logs = await _accessLogRepository.SearchAsync(
            request.PersonId, request.ProfessionalId, request.From, request.To, cancellationToken);

        var csv = new StringBuilder();
        csv.AppendLine("Data,Pessoa,Profissional,Acao,BaseLegal,Justificativa,CrossUnidade");

        foreach (var log in logs)
        {
            var response = await AccessLogResponseMapper.MapAsync(log, _personRepository, _professionalRepository, cancellationToken);
            csv.AppendLine(string.Join(',',
                EscapeCsvField(response.DateTime.ToString("O", CultureInfo.InvariantCulture)),
                EscapeCsvField(response.PersonName),
                EscapeCsvField(response.ProfessionalName),
                EscapeCsvField(response.Action),
                EscapeCsvField(response.LegalBasis),
                EscapeCsvField(response.Justification ?? string.Empty),
                EscapeCsvField(response.IsCrossUnit.ToString())));
        }

        return Result<string>.Success(csv.ToString());
    }

    private static string EscapeCsvField(string field)
    {
        if (field.Contains(',') || field.Contains('"') || field.Contains('\n'))
            return $"\"{field.Replace("\"", "\"\"")}\"";

        return field;
    }
}
