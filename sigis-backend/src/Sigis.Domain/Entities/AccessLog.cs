using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;
using Sigis.Domain.Events;

namespace Sigis.Domain.Entities;

/// <summary>
/// Registro de auditoria de um acesso a dados de uma pessoa por um
/// profissional, exigido pela LGPD (art. 11, II) sempre que o acesso ocorre
/// entre unidades diferentes. Agregado raiz do módulo de Auditoria de Acesso.
/// </summary>
public sealed class AccessLog : AggregateRoot
{
    /// <summary>Identificador da pessoa cujos dados foram acessados.</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Identificador do profissional que realizou o acesso.</summary>
    public Guid ProfessionalId { get; private set; }

    /// <summary>Ação de auditoria realizada (ex.: "ConsultaHistorico", "VisualizacaoTimeline").</summary>
    public string Action { get; private set; }

    /// <summary>Base legal (LGPD, art. 11) que amparou o acesso.</summary>
    public string LegalBasis { get; private set; }

    /// <summary>Justificativa do acesso, obrigatória quando o acesso é entre unidades.</summary>
    public string? Justification { get; private set; }

    /// <summary>Data e hora (UTC) do acesso.</summary>
    public DateTime DateTime { get; private set; }

    /// <summary>Endereço IP de origem do acesso, quando disponível.</summary>
    public string? Ip { get; private set; }

    /// <summary>User-Agent do cliente que realizou o acesso, quando disponível.</summary>
    public string? UserAgent { get; private set; }

    private readonly bool _isCrossUnit;

    private AccessLog(
        Guid id, Guid personId, Guid professionalId, string action, string legalBasis, string? justification,
        DateTime dateTime, bool isCrossUnit, string? ip, string? userAgent)
    {
        Id = id;
        PersonId = personId;
        ProfessionalId = professionalId;
        Action = action;
        LegalBasis = legalBasis;
        Justification = justification;
        DateTime = dateTime;
        _isCrossUnit = isCrossUnit;
        Ip = ip;
        UserAgent = userAgent;
    }

    /// <summary>
    /// Cria um novo registro de auditoria de acesso. Quando
    /// <paramref name="isCrossUnit"/> é <see langword="true"/>, exige
    /// justificativa (RNF02) e emite <see cref="CrossUnitAccessEvent"/>.
    /// </summary>
    /// <param name="personId">Identificador da pessoa cujos dados foram acessados.</param>
    /// <param name="professionalId">Identificador do profissional que realizou o acesso.</param>
    /// <param name="action">Ação de auditoria realizada.</param>
    /// <param name="legalBasis">Base legal (LGPD, art. 11) que amparou o acesso.</param>
    /// <param name="dateTime">Data e hora (UTC) do acesso.</param>
    /// <param name="isCrossUnit">
    /// Indica se o acesso cruza a fronteira de unidades (calculado pela
    /// camada de aplicação, que conhece a unidade do profissional e a origem
    /// dos dados acessados).
    /// </param>
    /// <param name="justification">Justificativa do acesso, obrigatória quando <paramref name="isCrossUnit"/> é verdadeiro.</param>
    /// <param name="ip">Endereço IP de origem, opcional.</param>
    /// <param name="userAgent">User-Agent do cliente, opcional.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso, ou de falha com
    /// <see cref="AccessLogErrors.AcaoInvalida"/>,
    /// <see cref="AccessLogErrors.BaseLegalObrigatoria"/> ou
    /// <see cref="AccessLogErrors.JustificativaObrigatoriaCrossUnidade"/>.
    /// </returns>
    public static Result<AccessLog> Create(
        Guid personId,
        Guid professionalId,
        string? action,
        string? legalBasis,
        DateTime dateTime,
        bool isCrossUnit,
        string? justification = null,
        string? ip = null,
        string? userAgent = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            return Result<AccessLog>.Failure(AccessLogErrors.AcaoInvalida);

        if (string.IsNullOrWhiteSpace(legalBasis))
            return Result<AccessLog>.Failure(AccessLogErrors.BaseLegalObrigatoria);

        if (isCrossUnit && string.IsNullOrWhiteSpace(justification))
            return Result<AccessLog>.Failure(AccessLogErrors.JustificativaObrigatoriaCrossUnidade);

        var accessLog = new AccessLog(
            Guid.NewGuid(), personId, professionalId, action.Trim(), legalBasis.Trim(), justification?.Trim(),
            dateTime, isCrossUnit, ip, userAgent);

        if (isCrossUnit)
            accessLog.Raise(new CrossUnitAccessEvent(accessLog.Id, personId, professionalId));

        return Result<AccessLog>.Success(accessLog);
    }

    /// <summary>Indica se este acesso cruzou a fronteira de unidades (RNF02, RF13).</summary>
    /// <returns>
    /// <see langword="true"/> quando o acesso foi classificado como
    /// cross-unidade no momento da criação — o cálculo em si é
    /// responsabilidade da camada de aplicação, que compara a unidade do
    /// profissional com a origem dos dados.
    /// </returns>
    public bool IsCrossUnit() => _isCrossUnit;
}
