import { SecretariaResponsavel } from "@/lib/types/enums";
import type { UnidadeServico } from "@/lib/types/unidade";

export const unidadesMock: UnidadeServico[] = [
  { id: "unidade-nasf", nome: "Nucleo Ampliado de Saude da Familia", sigla: "NASF", secretariaResponsavel: SecretariaResponsavel.SAUDE },
  { id: "unidade-creaes", nome: "Centro de Referencia em Educacao e Apoio Especial em Saude", sigla: "CREAES", secretariaResponsavel: SecretariaResponsavel.SAUDE },
  { id: "unidade-nape", nome: "Nucleo de Atendimento Pedagogico Especializado", sigla: "NAPE", secretariaResponsavel: SecretariaResponsavel.EDUCACAO },
  { id: "unidade-casa-mais-azul", nome: "Casa Mais Azul", sigla: "CASA_MAIS_AZUL", secretariaResponsavel: SecretariaResponsavel.SAUDE },
  { id: "unidade-crasf", nome: "Centro de Referencia de Assistencia Social e Familia", sigla: "CRASF", secretariaResponsavel: SecretariaResponsavel.ASSISTENCIA },
];
