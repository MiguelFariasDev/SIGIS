export type ConsentType = "Clinical" | "Educational" | "SocialAssistance" | "Research";

export interface PersonConsent {
  id: string;
  personId: string;
  type: ConsentType;
  granted: boolean;
  grantedAt: string;
  grantedByGuardianId?: string;
  revokedAt?: string;
  version: string;
  evidence?: string;
  createdAt: string;
}

export interface GrantConsentRequest {
  type: ConsentType;
  granted: boolean;
  grantedByGuardianId?: string;
  version: string;
  evidence?: string;
}

/** T17 — item da listagem global de consentimentos (painel DPO/COORDENADOR). */
export interface PersonConsentComPessoa extends PersonConsent {
  personName: string;
}

export interface ConsentimentosFiltros {
  personId?: string;
  type?: ConsentType;
  status?: "granted" | "revoked";
  dataInicio?: string;
  dataFim?: string;
}
