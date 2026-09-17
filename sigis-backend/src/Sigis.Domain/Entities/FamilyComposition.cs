using Sigis.Domain.Abstractions;
using Sigis.Domain.Abstractions.Errors;

namespace Sigis.Domain.Entities;

/// <summary>
/// Composição familiar de uma pessoa: dados dos pais, estrutura familiar e
/// informações sobre a gestação/concepção (NAPE, fichas A.2 §2 e A.3 §2-3).
/// Relação 1:1 com <see cref="Person"/>.
/// </summary>
public sealed class FamilyComposition : Entity
{
    private static readonly string[] ValidFiliationTypes = ["Natural", "Adotivo"];
    private static readonly string[] ValidDeliveryTypes = ["Normal", "Cesarea"];

    /// <summary>Identificador da pessoa.</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Nome do pai, quando informado.</summary>
    public string? FatherName { get; private set; }

    /// <summary>Escolaridade do pai, quando informada.</summary>
    public string? FatherEducation { get; private set; }

    /// <summary>Ocupação do pai, quando informada.</summary>
    public string? FatherOccupation { get; private set; }

    /// <summary>Nome da mãe, quando informado.</summary>
    public string? MotherName { get; private set; }

    /// <summary>Escolaridade da mãe, quando informada.</summary>
    public string? MotherEducation { get; private set; }

    /// <summary>Ocupação da mãe, quando informada.</summary>
    public string? MotherOccupation { get; private set; }

    /// <summary>Número de irmãos, quando informado.</summary>
    public int? SiblingsCount { get; private set; }

    /// <summary>Idades dos irmãos (texto livre), quando informadas.</summary>
    public string? SiblingsAges { get; private set; }

    /// <summary>Número de pessoas na residência, quando informado.</summary>
    public int? HouseholdMembersCount { get; private set; }

    /// <summary>Situação conjugal dos pais, quando informada.</summary>
    public string? ParentsMaritalStatus { get; private set; }

    /// <summary>Tipo de filiação ("Natural" ou "Adotivo"), quando informado.</summary>
    public string? FiliationType { get; private set; }

    /// <summary>Indica se a gravidez foi planejada, quando informado.</summary>
    public bool? PlannedPregnancy { get; private set; }

    /// <summary>Número de gestações da mãe, quando informado.</summary>
    public int? PregnanciesCount { get; private set; }

    /// <summary>Número de abortos da mãe, quando informado.</summary>
    public int? AbortionsCount { get; private set; }

    /// <summary>Problema de saúde relatado durante a gestação, quando informado.</summary>
    public string? PregnancyHealthIssue { get; private set; }

    /// <summary>Tipo de parto ("Normal" ou "Cesarea"), quando informado.</summary>
    public string? DeliveryType { get; private set; }

    /// <summary>Uso de medicamento durante a gravidez, quando informado.</summary>
    public string? MedicationDuringPregnancy { get; private set; }

    /// <summary>Data e hora (UTC) de criação do registro.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Data e hora (UTC) da última atualização do registro.</summary>
    public DateTime UpdatedAt { get; private set; }

    private FamilyComposition(Guid id, Guid personId, DateTime createdAt)
    {
        Id = id;
        PersonId = personId;
        CreatedAt = createdAt;
        UpdatedAt = createdAt;
    }

    /// <summary>
    /// Cria a composição familiar de uma pessoa. Todos os campos além de
    /// <paramref name="personId"/> são opcionais — cada seção da ficha pode
    /// ser preenchida aos poucos.
    /// </summary>
    /// <param name="personId">Identificador da pessoa (único — relação 1:1).</param>
    /// <param name="createdAt">Data e hora (UTC) de criação.</param>
    /// <param name="fatherName">Nome do pai, opcional.</param>
    /// <param name="fatherEducation">Escolaridade do pai, opcional.</param>
    /// <param name="fatherOccupation">Ocupação do pai, opcional.</param>
    /// <param name="motherName">Nome da mãe, opcional.</param>
    /// <param name="motherEducation">Escolaridade da mãe, opcional.</param>
    /// <param name="motherOccupation">Ocupação da mãe, opcional.</param>
    /// <param name="siblingsCount">Número de irmãos, opcional (RN-FC06: não negativo).</param>
    /// <param name="siblingsAges">Idades dos irmãos, opcional.</param>
    /// <param name="householdMembersCount">Pessoas na residência, opcional (RN-FC05: não negativo).</param>
    /// <param name="parentsMaritalStatus">Situação conjugal dos pais, opcional.</param>
    /// <param name="filiationType">Tipo de filiação ("Natural"/"Adotivo"), opcional (RN-FC03).</param>
    /// <param name="plannedPregnancy">Se a gravidez foi planejada, opcional.</param>
    /// <param name="pregnanciesCount">Número de gestações, opcional (RN-FC02, RN-FC05).</param>
    /// <param name="abortionsCount">Número de abortos, opcional (RN-FC02, RN-FC06).</param>
    /// <param name="pregnancyHealthIssue">Problema de saúde na gestação, opcional.</param>
    /// <param name="deliveryType">Tipo de parto ("Normal"/"Cesarea"), opcional (RN-FC04).</param>
    /// <param name="medicationDuringPregnancy">Medicação usada na gravidez, opcional.</param>
    /// <returns>
    /// Um <see cref="Result{T}"/> de sucesso, ou de falha quando alguma das
    /// regras de consistência (RN-FC02 a RN-FC06) for violada.
    /// </returns>
    public static Result<FamilyComposition> Create(
        Guid personId,
        DateTime createdAt,
        string? fatherName = null,
        string? fatherEducation = null,
        string? fatherOccupation = null,
        string? motherName = null,
        string? motherEducation = null,
        string? motherOccupation = null,
        int? siblingsCount = null,
        string? siblingsAges = null,
        int? householdMembersCount = null,
        string? parentsMaritalStatus = null,
        string? filiationType = null,
        bool? plannedPregnancy = null,
        int? pregnanciesCount = null,
        int? abortionsCount = null,
        string? pregnancyHealthIssue = null,
        string? deliveryType = null,
        string? medicationDuringPregnancy = null)
    {
        var validation = Validate(
            siblingsCount, householdMembersCount, filiationType, pregnanciesCount, abortionsCount, deliveryType);
        if (validation.IsFailure)
            return Result<FamilyComposition>.Failure(validation.Error);

        var composition = new FamilyComposition(Guid.NewGuid(), personId, createdAt)
        {
            FatherName = fatherName?.Trim(),
            FatherEducation = fatherEducation?.Trim(),
            FatherOccupation = fatherOccupation?.Trim(),
            MotherName = motherName?.Trim(),
            MotherEducation = motherEducation?.Trim(),
            MotherOccupation = motherOccupation?.Trim(),
            SiblingsCount = siblingsCount,
            SiblingsAges = siblingsAges?.Trim(),
            HouseholdMembersCount = householdMembersCount,
            ParentsMaritalStatus = parentsMaritalStatus?.Trim(),
            FiliationType = filiationType?.Trim(),
            PlannedPregnancy = plannedPregnancy,
            PregnanciesCount = pregnanciesCount,
            AbortionsCount = abortionsCount,
            PregnancyHealthIssue = pregnancyHealthIssue?.Trim(),
            DeliveryType = deliveryType?.Trim(),
            MedicationDuringPregnancy = medicationDuringPregnancy?.Trim(),
        };

        return Result<FamilyComposition>.Success(composition);
    }

    /// <summary>
    /// Atualiza os dados de pais e estrutura familiar (não altera os dados
    /// de concepção — ver <see cref="SetPregnancyInfo"/>).
    /// </summary>
    /// <returns>Um <see cref="Result"/> de sucesso, ou de falha quando irmãos/residentes forem negativos.</returns>
    public Result UpdateData(
        string? fatherName, string? fatherEducation, string? fatherOccupation,
        string? motherName, string? motherEducation, string? motherOccupation,
        int? siblingsCount, string? siblingsAges, int? householdMembersCount, string? parentsMaritalStatus)
    {
        if (siblingsCount is < 0)
            return Result.Failure(FamilyCompositionErrors.IrmaosNegativo);

        if (householdMembersCount is < 0)
            return Result.Failure(FamilyCompositionErrors.MembrosResidenciaNegativo);

        FatherName = fatherName?.Trim();
        FatherEducation = fatherEducation?.Trim();
        FatherOccupation = fatherOccupation?.Trim();
        MotherName = motherName?.Trim();
        MotherEducation = motherEducation?.Trim();
        MotherOccupation = motherOccupation?.Trim();
        SiblingsCount = siblingsCount;
        SiblingsAges = siblingsAges?.Trim();
        HouseholdMembersCount = householdMembersCount;
        ParentsMaritalStatus = parentsMaritalStatus?.Trim();
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    /// <summary>
    /// Atualiza os dados de concepção/gestação (NAPE A.3 §3), validando
    /// RN-FC02 (abortos ≤ gestações) e RN-FC03/RN-FC04.
    /// </summary>
    /// <returns>Um <see cref="Result"/> de sucesso, ou de falha quando alguma regra for violada.</returns>
    public Result SetPregnancyInfo(
        string? filiationType, bool? plannedPregnancy, int? pregnanciesCount, int? abortionsCount,
        string? pregnancyHealthIssue, string? deliveryType, string? medicationDuringPregnancy)
    {
        var validation = Validate(null, null, filiationType, pregnanciesCount, abortionsCount, deliveryType);
        if (validation.IsFailure)
            return validation;

        FiliationType = filiationType?.Trim();
        PlannedPregnancy = plannedPregnancy;
        PregnanciesCount = pregnanciesCount;
        AbortionsCount = abortionsCount;
        PregnancyHealthIssue = pregnancyHealthIssue?.Trim();
        DeliveryType = deliveryType?.Trim();
        MedicationDuringPregnancy = medicationDuringPregnancy?.Trim();
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    private static Result Validate(
        int? siblingsCount, int? householdMembersCount, string? filiationType,
        int? pregnanciesCount, int? abortionsCount, string? deliveryType)
    {
        if (siblingsCount is < 0)
            return Result.Failure(FamilyCompositionErrors.IrmaosNegativo);

        if (householdMembersCount is < 0)
            return Result.Failure(FamilyCompositionErrors.MembrosResidenciaNegativo);

        if (filiationType is not null && !ValidFiliationTypes.Contains(filiationType))
            return Result.Failure(FamilyCompositionErrors.TipoFiliacaoInvalido);

        if (deliveryType is not null && !ValidDeliveryTypes.Contains(deliveryType))
            return Result.Failure(FamilyCompositionErrors.TipoPartoInvalido);

        if (pregnanciesCount is < 0)
            return Result.Failure(FamilyCompositionErrors.GestacoesNegativas);

        if (abortionsCount is < 0)
            return Result.Failure(FamilyCompositionErrors.AbortosNegativos);

        if (pregnanciesCount.HasValue && abortionsCount.HasValue && abortionsCount > pregnanciesCount)
            return Result.Failure(FamilyCompositionErrors.AbortosMaiorQueGestacoes);

        return Result.Success();
    }
}
