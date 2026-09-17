import { PapelRbac } from "@/lib/types/enums";
import type { Profissional } from "@/lib/types/profissional";

/**
 * 3 contas de demonstracao (1 por papel RBAC) + profissionais adicionais
 * usados apenas como autores de atendimentos historicos (sem login).
 */
export const profissionaisMock: Profissional[] = [
  {
    id: "prof-profissional",
    nome: "Camila Rodrigues Teixeira",
    especialidade: "Terapeuta Ocupacional",
    unidadeId: "unidade-nasf",
    unidadeSigla: "NASF",
    papelRbac: PapelRbac.PROFISSIONAL,
    email: "camila.teixeira@crateus.ce.gov.br",
  },
  {
    id: "prof-coordenador",
    nome: "Roberto Carlos Meneses",
    especialidade: "Coordenacao da Rede de Cuidado TEA",
    unidadeId: "unidade-nasf",
    unidadeSigla: "NASF",
    papelRbac: PapelRbac.COORDENADOR,
    email: "roberto.meneses@crateus.ce.gov.br",
  },
  {
    id: "prof-auditor",
    nome: "Fernanda Lopes Cavalcante",
    especialidade: "DPO Municipal",
    unidadeId: "unidade-crasf",
    unidadeSigla: "CRASF",
    papelRbac: PapelRbac.AUDITOR,
    email: "fernanda.cavalcante@crateus.ce.gov.br",
  },
  {
    id: "prof-nape-psi",
    nome: "Juliana Pinheiro Andrade",
    especialidade: "Psicologa",
    unidadeId: "unidade-nape",
    unidadeSigla: "NAPE",
    papelRbac: PapelRbac.PROFISSIONAL,
    email: "juliana.andrade@crateus.ce.gov.br",
  },
  {
    id: "prof-nape-psicoped",
    nome: "Marcos Vinicius Bezerra",
    especialidade: "Psicopedagogo",
    unidadeId: "unidade-nape",
    unidadeSigla: "NAPE",
    papelRbac: PapelRbac.PROFISSIONAL,
    email: "marcos.bezerra@crateus.ce.gov.br",
  },
  {
    id: "prof-nape-edfisica",
    nome: "Patricia Gomes Nunes",
    especialidade: "Educadora Fisica",
    unidadeId: "unidade-nape",
    unidadeSigla: "NAPE",
    papelRbac: PapelRbac.PROFISSIONAL,
    email: "patricia.nunes@crateus.ce.gov.br",
  },
  {
    id: "prof-creaes",
    nome: "Diego Freitas Monteiro",
    especialidade: "Fonoaudiologo",
    unidadeId: "unidade-creaes",
    unidadeSigla: "CREAES",
    papelRbac: PapelRbac.PROFISSIONAL,
    email: "diego.monteiro@crateus.ce.gov.br",
  },
  {
    id: "prof-casa-azul",
    nome: "Larissa Cordeiro Viana",
    especialidade: "Psicologa",
    unidadeId: "unidade-casa-mais-azul",
    unidadeSigla: "CASA_MAIS_AZUL",
    papelRbac: PapelRbac.PROFISSIONAL,
    email: "larissa.viana@crateus.ce.gov.br",
  },
];
