import { formatarData } from "@/lib/utils/formatters";

export interface PessoaComparavel {
  fullName: string;
  birthDate: string;
  cns?: string;
  cpf?: string;
  motherName?: string;
  phone?: string;
  street?: string;
}

interface ComparacaoLadoALadoProps {
  tituloA: string;
  tituloB: string;
  pessoaA: PessoaComparavel;
  pessoaB: PessoaComparavel;
}

const CAMPOS: { chave: keyof PessoaComparavel; label: string; formatar?: (v: string) => string }[] = [
  { chave: "fullName", label: "Nome completo" },
  { chave: "birthDate", label: "Data de nascimento", formatar: (v) => formatarData(v) },
  { chave: "cns", label: "CNS" },
  { chave: "cpf", label: "CPF" },
  { chave: "motherName", label: "Nome da mae" },
  { chave: "phone", label: "Telefone" },
  { chave: "street", label: "Endereco" },
];

function Coluna({ titulo, pessoa, destacar }: { titulo: string; pessoa: PessoaComparavel; destacar: PessoaComparavel }) {
  return (
    <div className="flex-1 rounded-xl border border-border bg-white p-4">
      <p className="mb-3 text-sm font-semibold text-foreground">{titulo}</p>
      <dl className="space-y-2.5">
        {CAMPOS.map((campo) => {
          const valor = pessoa[campo.chave];
          const valorComparado = destacar[campo.chave];
          const divergente = valor && valorComparado && valor !== valorComparado;
          return (
            <div key={campo.chave}>
              <dt className="text-xs text-muted-foreground">{campo.label}</dt>
              <dd className={divergente ? "text-sm font-medium text-sus-yellow-dark" : "text-sm text-foreground"}>
                {valor ? (campo.formatar ? campo.formatar(valor) : valor) : "-"}
              </dd>
            </div>
          );
        })}
      </dl>
    </div>
  );
}

export function ComparacaoLadoALado({ tituloA, tituloB, pessoaA, pessoaB }: ComparacaoLadoALadoProps) {
  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
      <Coluna titulo={tituloA} pessoa={pessoaA} destacar={pessoaB} />
      <Coluna titulo={tituloB} pessoa={pessoaB} destacar={pessoaA} />
    </div>
  );
}
