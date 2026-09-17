using Sigis.Domain.Abstractions;

namespace Sigis.Domain.Entities;

/// <summary>
/// Perfil clínico de uma pessoa atendida pelo NASF (ficha A.1): número do
/// prontuário físico, hipótese diagnóstica e unidade de atenção primária
/// (APS) de referência. Relação 1:1 com <see cref="Person"/>.
/// </summary>
/// <remarks>
/// Regras que dependem de outras entidades — RN-CP02 (prontuário único por
/// unidade), RN-CP03 (H.D. obrigatória para quem tem atendimento em NASF) e
/// RN-CP04 (APS deve ser unidade da Secretaria de Saúde) — são
/// responsabilidade da camada de aplicação, que tem acesso aos repositórios
/// necessários para validá-las; a entidade isolada não pode verificá-las.
/// </remarks>
public sealed class PersonClinicalProfile : Entity
{
    /// <summary>Identificador da pessoa.</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Número do prontuário físico do NASF, quando informado.</summary>
    public string? MedicalRecordNumber { get; private set; }

    /// <summary>Hipótese diagnóstica (H.D.), quando informada.</summary>
    public string? ClinicalHypothesis { get; private set; }

    /// <summary>Identificador da unidade de atenção primária (APS) de referência, quando informada.</summary>
    public Guid? ApsReferenceUnitId { get; private set; }

    /// <summary>Data e hora (UTC) de criação do registro.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Data e hora (UTC) da última atualização do registro.</summary>
    public DateTime UpdatedAt { get; private set; }

    private PersonClinicalProfile(Guid id, Guid personId, DateTime createdAt)
    {
        Id = id;
        PersonId = personId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Cria o perfil clínico NASF de uma pessoa.
    /// </summary>
    /// <param name="personId">Identificador da pessoa (único — relação 1:1).</param>
    /// <param name="createdAt">Data e hora (UTC) de criação.</param>
    /// <param name="medicalRecordNumber">Número do prontuário físico, opcional.</param>
    /// <param name="clinicalHypothesis">Hipótese diagnóstica, opcional.</param>
    /// <param name="apsReferenceUnitId">Identificador da unidade APS de referência, opcional.</param>
    /// <returns>Um <see cref="Result{T}"/> sempre bem-sucedido com o perfil clínico criado.</returns>
    public static Result<PersonClinicalProfile> Create(
        Guid personId,
        DateTime createdAt,
        string? medicalRecordNumber = null,
        string? clinicalHypothesis = null,
        Guid? apsReferenceUnitId = null)
    {
        return Result<PersonClinicalProfile>.Success(new PersonClinicalProfile(Guid.NewGuid(), personId, createdAt)
        {
            MedicalRecordNumber = medicalRecordNumber?.Trim(),
            ClinicalHypothesis = clinicalHypothesis?.Trim(),
            ApsReferenceUnitId = apsReferenceUnitId,
        });
    }

    /// <summary>Atualiza os dados do perfil clínico.</summary>
    /// <returns>Um <see cref="Result"/> sempre bem-sucedido.</returns>
    public Result UpdateData(string? medicalRecordNumber, string? clinicalHypothesis, Guid? apsReferenceUnitId)
    {
        MedicalRecordNumber = medicalRecordNumber?.Trim();
        ClinicalHypothesis = clinicalHypothesis?.Trim();
        ApsReferenceUnitId = apsReferenceUnitId;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}
