namespace Sigis.Domain.Events;

/// <summary>
/// Evento disparado quando um profissional acessa dados de uma pessoa
/// originados em uma unidade diferente da sua própria (acesso cross-unidade),
/// relevante para auditoria e conformidade com a LGPD.
/// </summary>
/// <param name="AccessLogId">Identificador do registro de log de acesso.</param>
/// <param name="PersonId">Identificador da pessoa cujos dados foram acessados.</param>
/// <param name="ProfessionalId">Identificador do profissional que realizou o acesso.</param>
public sealed record CrossUnitAccessEvent(Guid AccessLogId, Guid PersonId, Guid ProfessionalId) : IDomainEvent
{
    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
