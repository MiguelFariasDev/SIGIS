namespace Sigis.Domain.Abstractions.Errors;

/// <summary>Erros do módulo de Autenticação e Autorização.</summary>
public static class AuthErrors
{
    /// <summary>Credenciais inválidas.</summary>
    public static readonly Error InvalidCredentials = new("AUTH_001", "Credenciais inválidas.");

    /// <summary>Usuário inativo.</summary>
    public static readonly Error UserInactive = new("AUTH_002", "Usuário inativo. Contate o administrador.");

    /// <summary>E-mail já cadastrado.</summary>
    public static readonly Error EmailAlreadyExists = new("AUTH_003", "E-mail já cadastrado.");

    /// <summary>Senha fraca.</summary>
    public static readonly Error WeakPassword = new("AUTH_004", "Senha deve ter ao menos 8 caracteres, com letras e números.");

    /// <summary>Sessão expirada.</summary>
    public static readonly Error TokenExpired = new("AUTH_005", "Sessão expirada. Faça login novamente.");

    /// <summary>Acesso não autorizado.</summary>
    public static readonly Error Unauthorized = new("AUTH_006", "Acesso não autorizado.");

    /// <summary>Sem permissão para esta ação.</summary>
    public static readonly Error Forbidden = new("AUTH_007", "Você não tem permissão para esta ação.");
}
