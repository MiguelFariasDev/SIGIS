using Sigis.Domain.Entities;

namespace Sigis.Domain.Interfaces;

/// <summary>Repositório de persistência para o agregado <see cref="Attendance"/>.</summary>
public interface IAttendanceRepository
{
    /// <summary>Busca um atendimento pelo identificador.</summary>
    /// <param name="id">Identificador do atendimento.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>O atendimento encontrado, ou <see langword="null"/> quando não existir.</returns>
    Task<Attendance?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    /// <summary>
    /// Lista o histórico completo de atendimentos de uma pessoa em todas as
    /// unidades da rede — base da timeline consolidada (RF08).
    /// </summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de atendimentos da pessoa, podendo ser vazia.</returns>
    Task<IReadOnlyList<Attendance>> GetByPersonAsync(Guid personId, CancellationToken cancellationToken);

    /// <summary>
    /// Verifica se a pessoa possui algum atendimento ativo (agendado) em
    /// outra unidade que não a informada — usado para alertar duplicidade de
    /// atendimento (RF10).
    /// </summary>
    /// <param name="personId">Identificador da pessoa.</param>
    /// <param name="excludingUnitId">Identificador da unidade a ser excluída da verificação.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns><see langword="true"/> quando existir atendimento ativo em outra unidade.</returns>
    Task<bool> HasActiveAttendanceInOtherUnitAsync(
        Guid personId, Guid excludingUnitId, CancellationToken cancellationToken);

    /// <summary>Adiciona um novo atendimento ao repositório.</summary>
    /// <param name="attendance">Atendimento a ser adicionado.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task AddAsync(Attendance attendance, CancellationToken cancellationToken);

    /// <summary>
    /// Busca atendimentos filtrados opcionalmente por unidade e período —
    /// usado pelo painel de indicadores.
    /// </summary>
    /// <param name="unitId">Identificador da unidade de serviço, opcional.</param>
    /// <param name="from">Início do período (UTC, inclusive), opcional.</param>
    /// <param name="to">Fim do período (UTC, inclusive), opcional.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Lista de atendimentos que atendem aos filtros informados, podendo ser vazia.</returns>
    Task<IReadOnlyList<Attendance>> SearchAsync(
        Guid? unitId, DateTime? from, DateTime? to, CancellationToken cancellationToken);

    /// <summary>Atualiza os dados de um atendimento já existente no repositório.</summary>
    /// <param name="attendance">Atendimento com os dados atualizados.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    Task UpdateAsync(Attendance attendance, CancellationToken cancellationToken);

    /// <summary>
    /// Reatribui todos os atendimentos de uma pessoa para outra, em massa —
    /// usado na mesclagem de cadastros duplicados (RN02).
    /// </summary>
    /// <param name="fromPersonId">Identificador do cadastro de origem.</param>
    /// <param name="toPersonId">Identificador do cadastro de destino.</param>
    /// <param name="cancellationToken">Token de cancelamento da operação.</param>
    /// <returns>Quantidade de atendimentos reatribuídos.</returns>
    Task<int> ReassignPersonAsync(Guid fromPersonId, Guid toPersonId, CancellationToken cancellationToken);
}
