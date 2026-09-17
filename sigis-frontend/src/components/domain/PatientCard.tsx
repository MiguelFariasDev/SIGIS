import { User } from "lucide-react";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import type { Person } from "@/lib/types/person";
import { formatarData, formatarIdade } from "@/lib/utils/formatters";
import { ServiceChip } from "./ServiceChip";

interface PatientCardProps {
  pessoa: Pick<Person, "fullName" | "birthDate" | "cns" | "services">;
  onClick?: () => void;
}

function iniciais(nome: string): string {
  const partes = nome.trim().split(/\s+/);
  return `${partes[0]?.[0] ?? ""}${partes[partes.length - 1]?.[0] ?? ""}`.toUpperCase();
}

export function PatientCard({ pessoa, onClick }: PatientCardProps) {
  return (
    <button
      type="button"
      onClick={onClick}
      className="flex w-full items-center gap-3 rounded-xl border border-border bg-white p-4 text-left transition-colors hover:border-sus-blue/40 hover:bg-sus-blue-light/40"
    >
      <Avatar className="size-11 shrink-0">
        <AvatarFallback className="bg-sus-blue-light text-sus-blue-dark">
          {pessoa.fullName ? iniciais(pessoa.fullName) : <User className="size-5" />}
        </AvatarFallback>
      </Avatar>
      <div className="min-w-0 flex-1">
        <p className="truncate font-medium text-foreground">{pessoa.fullName}</p>
        <p className="text-xs text-muted-foreground">
          {formatarIdade(pessoa.birthDate)} — nasc. {formatarData(pessoa.birthDate)}
          {pessoa.cns && ` — CNS ${pessoa.cns}`}
        </p>
        {pessoa.services && pessoa.services.length > 0 && (
          <div className="mt-2 flex flex-wrap gap-1.5">
            {pessoa.services.map((servico) => (
              <ServiceChip key={servico} sigla={servico} />
            ))}
          </div>
        )}
      </div>
    </button>
  );
}
