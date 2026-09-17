using Sigis.Domain.Entities;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para a entidade <see cref="PersonClinicalProfile"/> (1:1 com pessoa).</summary>
public interface IPersonClinicalProfileRepository
{
    /// <summary>Busca o perfil clínico de uma pessoa.</summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O perfil clínico, ou <see langword="null"/> quando ainda não cadastrado.</returns>
    Task<PersonClinicalProfile?> GetByPersonAsync(Guid personId, CancellationToken cancellationToken);

    /// <summary>
    /// Verifica se já existe um número de prontuário cadastrado para a
    /// unidade APS informada (RN-CP02).
    /// </summary>
    /// <param name="apsReferenceUnitId">Identificador da unidade APS.</param>
    /// <param name="medicalRecordNumber">Número do prontuário.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns><see langword="true"/> quando já existir um prontuário com esse número na unidade.</returns>
    Task<bool> ExistsMedicalRecordNumberInUnitAsync(
        Guid apsReferenceUnitId, string medicalRecordNumber, CancellationToken cancellationToken);

    /// <summary>Adiciona o perfil clínico de uma pessoa.</summary>
    /// <param name="profile">Perfil clínico a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(PersonClinicalProfile profile, CancellationToken cancellationToken);

    /// <summary>Atualiza o perfil clínico já existente no repositório.</summary>
    /// <param name="profile">Perfil clínico com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(PersonClinicalProfile profile, CancellationToken cancellationToken);
}
