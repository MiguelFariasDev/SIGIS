import type { LegalBasis } from "@/lib/types/legalBasis";

export const legalBasisMock: LegalBasis[] = [
  {
    id: "base-legal-01",
    code: "ART11-II-A",
    description: "Tutela da saude, em procedimento realizado por servico de saude",
    secretariat: "Health",
    purpose: "Atendimento clinico e continuidade do cuidado",
    article: "LGPD art. 11, II, 'a'",
  },
  {
    id: "base-legal-02",
    code: "ART11-II-F",
    description: "Exercicio regular de direitos em contrato/processo administrativo",
    secretariat: "Health",
    purpose: "Registro de sessao e comparecimento",
    article: "LGPD art. 11, II, 'f'",
  },
  {
    id: "base-legal-03",
    code: "ART11-II-EDU",
    description: "Tutela da saude/educacao, procedimento por servico de educacao especializada",
    secretariat: "Education",
    purpose: "Acompanhamento pedagogico especializado (NAPE)",
    article: "LGPD art. 11, II, 'a'",
  },
  {
    id: "base-legal-04",
    code: "ART11-II-ASSIST",
    description: "Tutela da saude, procedimento por servico de assistencia social",
    secretariat: "SocialAssistance",
    purpose: "Acompanhamento socioassistencial familiar",
    article: "LGPD art. 11, II, 'a'",
  },
];
