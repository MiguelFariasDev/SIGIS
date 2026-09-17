import { StatusComparecimento } from "@/lib/types/enums";
import { COMPARECIMENTO_LABEL } from "@/lib/utils/constants";
import { cn } from "@/lib/utils";

interface ComparecimentoToggleProps {
  value: StatusComparecimento;
  onChange: (value: StatusComparecimento) => void;
}

const OPCOES = [StatusComparecimento.AGENDADO, StatusComparecimento.COMPARECEU, StatusComparecimento.FALTOU];

const CORES: Record<StatusComparecimento, string> = {
  AGENDADO: "data-[active=true]:bg-sus-blue data-[active=true]:text-white data-[active=true]:border-sus-blue",
  COMPARECEU: "data-[active=true]:bg-sus-green data-[active=true]:text-white data-[active=true]:border-sus-green",
  FALTOU: "data-[active=true]:bg-sus-red data-[active=true]:text-white data-[active=true]:border-sus-red",
};

export function ComparecimentoToggle({ value, onChange }: ComparecimentoToggleProps) {
  return (
    <div className="inline-flex rounded-lg border border-border bg-white p-1">
      {OPCOES.map((opcao) => (
        <button
          key={opcao}
          type="button"
          data-active={value === opcao}
          onClick={() => onChange(opcao)}
          className={cn(
            "rounded-md border border-transparent px-3 py-1.5 text-sm font-medium text-muted-foreground transition-colors",
            CORES[opcao],
          )}
        >
          {COMPARECIMENTO_LABEL[opcao]}
        </button>
      ))}
    </div>
  );
}
