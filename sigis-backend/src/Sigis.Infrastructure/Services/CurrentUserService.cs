using Microsoft.AspNetCore.Http;
using Sigis.Domain.Enums;
using Sigis.Domain.Interfaces;

namespace Sigis.Infrastructure.Services;

/// <summary>
/// Implementação de <see cref="ICurrentUserService"/> que lê os dados do
/// profissional autenticado a partir das claims do <see cref="HttpContext"/>
/// atual. A autenticação JWT propriamente dita é configurada em um prompt
/// futuro (Sigis.Api) — este serviço apenas projeta as claims já presentes
/// no <see cref="System.Security.Claims.ClaimsPrincipal"/> em um contrato
/// puro que o domínio e a aplicação podem consumir sem depender de
/// ASP.NET Core diretamente.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private const string ProfessionalIdClaim = "professional_id";
    private const string UnitIdClaim = "unit_id";
    private const string RoleClaim = "role";

    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>Cria o serviço de usuário atual.</summary>
    /// <param name="httpContextAccessor">Acessor do contexto HTTP da requisição atual.</param>
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public Guid? ProfessionalId => TryGetGuidClaim(ProfessionalIdClaim);

    /// <inheritdoc />
    public Guid? UnitId => TryGetGuidClaim(UnitIdClaim);

    /// <inheritdoc />
    public RbacRole? Role
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirst(RoleClaim)?.Value;
            return Enum.TryParse<RbacRole>(value, ignoreCase: true, out var role) ? role : null;
        }
    }

    /// <inheritdoc />
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    private Guid? TryGetGuidClaim(string claimType)
    {
        var value = _httpContextAccessor.HttpContext?.User?.FindFirst(claimType)?.Value;
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
