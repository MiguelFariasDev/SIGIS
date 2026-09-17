namespace Sigis.Domain.Enums;

/// <summary>Papel de controle de acesso (RBAC) de um profissional no sistema.</summary>
public enum RbacRole
{
    /// <summary>Profissional de atendimento — escopo restrito à própria unidade.</summary>
    Professional = 1,

    /// <summary>Coordenador — escopo de rede completa, resolve duplicidades.</summary>
    Coordinator = 2,

    /// <summary>Auditor — acesso somente leitura aos logs de acesso.</summary>
    Auditor = 3
}
