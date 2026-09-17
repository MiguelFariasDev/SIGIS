using Sigis.Domain.Entities;
using Sigis.Domain.ValueObjects;

namespace Sigis.Domain.Interfaces;

/// <summary>
/// Repositório de acesso a <see cref="Professional"/> voltado à autenticação
/// (busca por e-mail de login). Separado de <see cref="IProfessionalRepository"/>
/// por representar uma responsabilidade distinta (identidade/autenticação
/// vs. cadastro de profissionais da rede).
/// </summary>
public interface IUserRepository
{
    /// <summary>Busca um profissional pelo e-mail de login.</summary>
    /// <param name="email">E-mail de login.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O profissional encontrado, ou <see langword="null"/> quando não existir.</returns>
    Task<Professional?> GetByEmailAsync(EmailAddress email, CancellationToken cancellationToken);

    /// <summary>Busca um profissional pelo identificador.</summary>
    /// <param name="id">Identificador do profissional.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O profissional encontrado, ou <see langword="null"/> quando não existir.</returns>
    Task<Professional?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Atualiza os dados de autenticação de um profissional (ex.: último login).</summary>
    /// <param name="professional">Profissional com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(Professional professional, CancellationToken cancellationToken);
}
