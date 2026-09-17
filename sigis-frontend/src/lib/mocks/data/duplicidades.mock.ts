import { StatusAlertaDuplicidade } from "@/lib/types/enums";
import type { AlertaDuplicidade } from "@/lib/types/duplicidade";
import { pessoasMock } from "./pessoas.mock";

export const duplicidadesMock: AlertaDuplicidade[] = [
  {
    id: "duplicidade-01",
    personId1: "pessoa-08",
    personId2: "pessoa-09",
    person1: pessoasMock.find((p) => p.id === "pessoa-08"),
    person2: pessoasMock.find((p) => p.id === "pessoa-09"),
    scoreSimilaridade: 0.87,
    motivoMatch: "Mesma data de nascimento (15/12/2012) + nome 87% similar + mesmo endereco.",
    status: StatusAlertaDuplicidade.PENDENTE,
    criadoEm: "2026-02-19T16:46:00Z",
  },
];
