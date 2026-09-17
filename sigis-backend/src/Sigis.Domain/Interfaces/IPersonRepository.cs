using Sigis.Domain.Entities;
using Sigis.Domain.ValueObjects;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para o agregado <see cref="Person"/>.</summary>
public interface IPersonRepository
{
    /// <summary>Busca uma pessoa pelo identificador.</summary>
    /// <param name="id">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>A pessoa encontrada, ou <see langword="null"/> quando não existir.</returns>
    Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>Busca uma pessoa pelo número do CNS.</summary>
    /// <param name="cns">Número do CNS.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>A pessoa encontrada, ou <see langword="null"/> quando não existir.</returns>
    Task<Person?> GetByCnsAsync(Cns cns, CancellationToken cancellationToken);

    /// <summary>Busca uma pessoa pelo número do CPF.</summary>
    /// <param name="cpf">Número do CPF.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>A pessoa encontrada, ou <see langword="null"/> quando não existir.</returns>
    Task<Person?> GetByCpfAsync(Cpf cpf, CancellationToken cancellationToken);

    /// <summary>Busca pessoas cujo nome corresponda (parcial ou aproximadamente) ao termo informado.</summary>
    /// <param name="term">Termo de busca por nome.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de pessoas encontradas, podendo ser vazia.</returns>
    Task<IReadOnlyList<Person>> SearchByNameAsync(string term, CancellationToken cancellationToken);

    /// <summary>
    /// Busca candidatos a duplicidade de cadastro por nome aproximado e data
    /// de nascimento exata (RN02 — camada 3, matching probabilístico via
    /// <c>pg_trgm</c> na infraestrutura).
    /// </summary>
    /// <param name="name">Nome a ser comparado.</param>
    /// <param name="birthDate">Data de nascimento exata a ser filtrada.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de pessoas candidatas a duplicidade, podendo ser vazia.</returns>
    Task<IReadOnlyList<Person>> FindDuplicateCandidatesAsync(
        PersonName name, DateOnly birthDate, CancellationToken cancellationToken);

    /// <summary>Adiciona uma nova pessoa ao repositório.</summary>
    /// <param name="person">Pessoa a ser adicionada.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(Person person, CancellationToken cancellationToken);

    /// <summary>Atualiza os dados de uma pessoa já existente no repositório.</summary>
    /// <param name="person">Pessoa com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(Person person, CancellationToken cancellationToken);
}
