namespace Sigis.Domain.Enums;

/// <summary>Tipo de sessão/anamnese registrada em um atendimento, específico de cada serviço da rede.</summary>
public enum SessionType
{
    /// <summary>Anamnese psicológica (NAPE).</summary>
    PsychologicalAnamnesis = 1,

    /// <summary>Anamnese psicopedagógica (NAPE).</summary>
    PsychopedagogicalAnamnesis = 2,

    /// <summary>Instrumental de avaliação de educação física (NAPE).</summary>
    PhysicalEducationInstrument = 3,

    /// <summary>Prontuário clínico simples (NASF).</summary>
    NasfRecord = 4,

    /// <summary>Síntese de acompanhamento (NAPE).</summary>
    Synthesis = 5
}
