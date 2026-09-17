namespace Sigis.Domain.Interfaces;

/// <summary>
/// Marca um agregado como auditável por data de última atualização, para que
/// a infraestrutura de persistência (ver <c>AuditInterceptor</c> em
/// <c>Sigis.Infrastructure</c>) possa atualizar automaticamente o carimbo de
/// tempo a cada modificação, sem que o próprio domínio precise conhecer o
/// mecanismo de persistência.
/// </summary>
/// <remarks>
/// A criação (<c>CreatedAt</c>) já é responsabilidade do próprio construtor
/// de domínio de cada agregado (ver <c>Person.Create</c>, por exemplo) e não
/// depende de infraestrutura. Apenas <see cref="UpdatedAt"/> — que precisa
/// ser tocado a cada alteração, inclusive as feitas fora de métodos de
/// domínio específicos — é responsabilidade desta interface.
/// </remarks>
public interface IAuditable
{
    /// <summary>Data e hora (UTC) da última atualização do agregado.</summary>
    DateTime UpdatedAt { get; }
}
