namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros do módulo de Histórico Escolar (<c>SchoolHistory</c>).</summary>
public static class SchoolHistoryErrors
{
    /// <summary>Nome da escola é obrigatório.</summary>
    public static readonly Error NomeEscolaObrigatorio = new("SCHOOL_001", "Nome da escola é obrigatório.");

    /// <summary>Série é obrigatória.</summary>
    public static readonly Error SerieObrigatoria = new("SCHOOL_002", "Série é obrigatória.");

    /// <summary>Já existe uma escola ativa para este aluno.</summary>
    public static readonly Error EscolaAtivaJaExiste = new("SCHOOL_003", "Já existe uma escola ativa para este aluno.", ErrorType.Conflict);

    /// <summary>Ano letivo inválido (deve estar entre 1900 e o ano atual).</summary>
    public static readonly Error AnoLetivoInvalido = new("SCHOOL_004", "Ano letivo inválido (deve estar entre 1900 e o ano atual).");

    /// <summary>Data de término deve ser posterior à data de início.</summary>
    public static readonly Error DataTerminoInvalida = new("SCHOOL_005", "Data de término deve ser posterior à data de início.");

    /// <summary>Data de término só pode ser preenchida quando o status não for Ativo (RN-SH02).</summary>
    public static readonly Error DataTerminoApenasQuandoInativo = new("SCHOOL_006", "Data de término só pode ser preenchida quando o registro não estiver ativo.");
}
