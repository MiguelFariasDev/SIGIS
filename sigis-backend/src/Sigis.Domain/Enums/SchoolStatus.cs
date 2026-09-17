namespace Sigis.Domain.Enums;

/// <summary>Situação de um registro de histórico escolar de uma pessoa.</summary>
public enum SchoolStatus
{
    /// <summary>Matrícula ativa na escola.</summary>
    Ativo = 1,

    /// <summary>Aluno transferido para outra escola.</summary>
    Transferido = 2,

    /// <summary>Curso/série concluído nesta escola.</summary>
    Concluido = 3,

    /// <summary>Aluno evadido (abandono escolar).</summary>
    Evadido = 4
}
