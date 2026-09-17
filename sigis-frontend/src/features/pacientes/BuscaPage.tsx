import { Search, UserPlus } from "lucide-react";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { PageHeader } from "@/components/common/PageHeader";
import { LoadingState } from "@/components/common/LoadingState";
import { ErrorState } from "@/components/common/ErrorState";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import type { PersonSearchFilters } from "@/lib/types/person";
import { FiltrosBusca } from "./components/FiltrosBusca";
import { ResultadoBusca } from "./components/ResultadoBusca";
import { useBuscarPessoas } from "./hooks";

export function BuscaPage() {
  const navigate = useNavigate();
  const [filtros, setFiltros] = useState<PersonSearchFilters>({});
  const [termo, setTermo] = useState("");

  const buscaQuery = useBuscarPessoas({ ...filtros, term: termo });

  return (
    <div>
      <PageHeader
        title="Buscar paciente"
        description="Localize um paciente pelo nome, CNS ou CPF para consultar historico e fila."
        actions={
          <Button className="bg-sus-blue hover:bg-sus-blue-dark" onClick={() => navigate("/pacientes/novo")}>
            <UserPlus className="size-4" />
            Novo cadastro
          </Button>
        }
      />

      <div className="mb-4 space-y-3">
        <div className="relative max-w-xl">
          <Search className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            value={termo}
            onChange={(e) => setTermo(e.target.value)}
            placeholder="Buscar por nome, CNS ou CPF..."
            className="pl-9 text-base"
          />
        </div>
        <FiltrosBusca filtros={filtros} onChange={setFiltros} />
      </div>

      {buscaQuery.isLoading && <LoadingState rows={5} />}
      {buscaQuery.isError && <ErrorState onRetry={() => buscaQuery.refetch()} />}
      {buscaQuery.data && <ResultadoBusca resultados={buscaQuery.data} />}
    </div>
  );
}
