using Sigis.Domain.Enums;

namespace Sigis.Application.UseCases.LegalBasis;

/// <summary>Base legal (LGPD) que ampara o tratamento de dados por uma secretaria responsável.</summary>
/// <param name="Secretariat">Secretaria a que esta base legal se aplica.</param>
/// <param name="Article">Dispositivo da LGPD (Lei 13.709/2018) que ampara o tratamento.</param>
/// <param name="Justification">Justificativa, em português, de por que o tratamento é lícito para esta secretaria.</param>
public sealed record LegalBasisResponse(ResponsibleSecretariat Secretariat, string Article, string Justification);
