using Sigis.Domain.Enums;

namespace Sigis.Domain.Interfaces;

/// <summary>
/// Expõe informações do profissional autenticado na requisição atual,
/// usadas para aplicar RBAC (RF12) e registrar auditoria de acesso (RF13)
/// sem que o domínio dependa diretamente do mecanismo de autenticação.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>Identificador do profissional autenticado, ou <see langword="null"/> quando anônimo.</summary>
    Guid? ProfessionalId { get; }

    /// <summary>Identificador da unidade de serviço do profissional autenticado, ou <see langword="null"/> quando anônimo.</summary>
    Guid? UnitId { get; }

    /// <summary>Papel de controle de acesso (RBAC) do profissional autenticado, ou <see langword="null"/> quando anônimo.</summary>
    RbacRole? Role { get; }

    /// <summary>Indica se a requisição atual está autenticada.</summary>
    bool IsAuthenticated { get; }
}
