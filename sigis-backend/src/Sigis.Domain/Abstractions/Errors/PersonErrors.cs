namespace Sigis.Domain.Abstractions.Errors;

/// <summary>
/// Erros do módulo de Cadastro Único: validação da identidade da pessoa
/// (<c>Person</c>), de seus responsáveis (<c>Guardian</c>), das unidades de
/// serviço (<c>ServiceUnit</c>) e dos profissionais (<c>Professional</c>), e
/// dos value objects associados (nome, CNS, CPF, telefone, e-mail, endereço).
/// </summary>
public static class PersonErrors
{
    /// <summary>Nome completo é obrigatório.</summary>
    public static readonly Error NomeVazio = new("PERSON_001", "Nome completo é obrigatório.");

    /// <summary>Nome completo deve ter ao menos 5 caracteres.</summary>
    public static readonly Error NomeMuitoCurto = new("PERSON_002", "Nome completo deve ter ao menos 5 caracteres.");

    /// <summary>Nome completo não pode exceder 150 caracteres.</summary>
    public static readonly Error NomeMuitoLongo = new("PERSON_003", "Nome completo não pode exceder 150 caracteres.");

    /// <summary>Nome completo deve ter ao menos duas palavras.</summary>
    public static readonly Error NomeUmaPalavra = new("PERSON_004", "Nome completo deve ter ao menos duas palavras.");

    /// <summary>Cada palavra do nome deve ter ao menos 2 caracteres.</summary>
    public static readonly Error NomePalavraCurta = new("PERSON_005", "Cada palavra do nome deve ter ao menos 2 caracteres.");

    /// <summary>Nome completo contém caracteres inválidos.</summary>
    public static readonly Error NomeInvalido = new("PERSON_006", "Nome completo contém caracteres inválidos.");

    /// <summary>Data de nascimento não pode ser futura.</summary>
    public static readonly Error DataNascimentoFutura = new("PERSON_007", "Data de nascimento não pode ser futura.");

    /// <summary>Data de nascimento inválida (anterior a 1900).</summary>
    public static readonly Error DataNascimentoMuitoAntiga = new("PERSON_008", "Data de nascimento inválida (anterior a 1900).");

    /// <summary>CNS inválido.</summary>
    public static readonly Error CnsInvalido = new("PERSON_009", "CNS inválido.");

    /// <summary>CPF inválido.</summary>
    public static readonly Error CpfInvalido = new("PERSON_010", "CPF inválido.");

    /// <summary>Telefone inválido.</summary>
    public static readonly Error TelefoneInvalido = new("PERSON_011", "Telefone inválido.");

    /// <summary>E-mail inválido.</summary>
    public static readonly Error EmailInvalido = new("PERSON_012", "E-mail inválido.");

    /// <summary>Endereço inválido.</summary>
    public static readonly Error EnderecoInvalido = new("PERSON_013", "Endereço inválido.");

    /// <summary>Pessoa não encontrada.</summary>
    public static readonly Error PessoaNaoEncontrada = new("PERSON_014", "Pessoa não encontrada.", ErrorType.NotFound);

    /// <summary>Já existe pessoa com este CNS.</summary>
    public static readonly Error CnsDuplicado = new("PERSON_015", "Já existe pessoa com este CNS.", ErrorType.Conflict);

    /// <summary>Já existe pessoa com este CPF.</summary>
    public static readonly Error CpfDuplicado = new("PERSON_016", "Já existe pessoa com este CPF.", ErrorType.Conflict);

    /// <summary>Relacionamento do responsável com a pessoa é obrigatório.</summary>
    public static readonly Error RelacionamentoObrigatorio = new("PERSON_017", "Relacionamento do responsável com a pessoa é obrigatório.");

    /// <summary>Responsável não encontrado.</summary>
    public static readonly Error ResponsavelNaoEncontrado = new("PERSON_018", "Responsável não encontrado.", ErrorType.NotFound);

    /// <summary>Nome da unidade de serviço é obrigatório.</summary>
    public static readonly Error NomeUnidadeObrigatorio = new("PERSON_019", "Nome da unidade de serviço é obrigatório.");

    /// <summary>Sigla da unidade de serviço é obrigatória.</summary>
    public static readonly Error SiglaUnidadeObrigatoria = new("PERSON_020", "Sigla da unidade de serviço é obrigatória.");

    /// <summary>Especialidade do profissional é obrigatória.</summary>
    public static readonly Error EspecialidadeProfissionalObrigatoria = new("PERSON_021", "Especialidade do profissional é obrigatória.");

    /// <summary>Unidade do profissional é obrigatória.</summary>
    public static readonly Error UnidadeProfissionalObrigatoria = new("PERSON_022", "Unidade do profissional é obrigatória.");

    /// <summary>Origem e destino da mesclagem não podem ser a mesma pessoa.</summary>
    public static readonly Error MesclagemMesmaPessoa = new("PERSON_023", "Origem e destino da mesclagem não podem ser a mesma pessoa.");
}
