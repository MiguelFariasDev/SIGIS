namespace Sigis.Application.UseCases.Auth;

/// <summary>Resposta de um login bem-sucedido, com o token de acesso e o resumo do profissional autenticado.</summary>
/// <param name="AccessToken">Token JWT de acesso.</param>
/// <param name="ExpiresIn">Tempo de vida do token, em segundos.</param>
/// <param name="User">Resumo dos dados do profissional autenticado.</param>
public sealed record LoginResponse(string AccessToken, int ExpiresIn, LoginUserSummary User);

/// <summary>Resumo do profissional autenticado, sem dados sensíveis (nunca inclui o hash de senha).</summary>
/// <param name="Id">Identificador do profissional.</param>
/// <param name="Name">Nome completo.</param>
/// <param name="Email">E-mail de login.</param>
/// <param name="Role">Papel de controle de acesso (RBAC).</param>
/// <param name="UnitId">Identificador da unidade de serviço.</param>
/// <param name="UnitName">Nome da unidade de serviço.</param>
/// <param name="UnitAcronym">Sigla da unidade de serviço.</param>
/// <param name="Secretariat">Secretaria responsável pela unidade.</param>
public sealed record LoginUserSummary(
    Guid Id,
    string Name,
    string Email,
    string Role,
    Guid UnitId,
    string UnitName,
    string UnitAcronym,
    string Secretariat);
