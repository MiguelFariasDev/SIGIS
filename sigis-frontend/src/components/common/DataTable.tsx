import { ArrowDown, ArrowUp, ArrowUpDown, ChevronLeft, ChevronRight } from "lucide-react";
import { type ReactNode, useMemo, useState } from "react";
import { Button } from "@/components/ui/button";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { EmptyState } from "./EmptyState";

export interface DataTableColumn<T> {
  key: string;
  header: string;
  render: (row: T) => ReactNode;
  sortValue?: (row: T) => string | number;
  className?: string;
}

interface DataTableProps<T> {
  columns: DataTableColumn<T>[];
  data: T[];
  rowKey: (row: T) => string;
  onRowClick?: (row: T) => void;
  pageSize?: number;
  emptyTitle?: string;
  emptyDescription?: string;
}

export function DataTable<T>({
  columns,
  data,
  rowKey,
  onRowClick,
  pageSize = 10,
  emptyTitle = "Nenhum registro encontrado",
  emptyDescription,
}: DataTableProps<T>) {
  const [pagina, setPagina] = useState(0);
  const [ordenacao, setOrdenacao] = useState<{ chave: string; direcao: "asc" | "desc" } | null>(null);

  const dadosOrdenados = useMemo(() => {
    if (!ordenacao) return data;
    const coluna = columns.find((c) => c.key === ordenacao.chave);
    if (!coluna?.sortValue) return data;
    const copia = [...data];
    copia.sort((a, b) => {
      const va = coluna.sortValue!(a);
      const vb = coluna.sortValue!(b);
      const comparacao = va < vb ? -1 : va > vb ? 1 : 0;
      return ordenacao.direcao === "asc" ? comparacao : -comparacao;
    });
    return copia;
  }, [data, ordenacao, columns]);

  const totalPaginas = Math.max(1, Math.ceil(dadosOrdenados.length / pageSize));
  const paginaAtual = Math.min(pagina, totalPaginas - 1);
  const dadosPagina = dadosOrdenados.slice(paginaAtual * pageSize, paginaAtual * pageSize + pageSize);

  function alternarOrdenacao(coluna: DataTableColumn<T>) {
    if (!coluna.sortValue) return;
    setOrdenacao((atual) => {
      if (atual?.chave !== coluna.key) return { chave: coluna.key, direcao: "asc" };
      if (atual.direcao === "asc") return { chave: coluna.key, direcao: "desc" };
      return null;
    });
  }

  if (data.length === 0) {
    return <EmptyState title={emptyTitle} description={emptyDescription} />;
  }

  return (
    <div className="overflow-hidden rounded-xl border border-border bg-white">
      <Table>
        <TableHeader>
          <TableRow className="hover:bg-transparent">
            {columns.map((coluna) => (
              <TableHead key={coluna.key} className={coluna.className}>
                {coluna.sortValue ? (
                  <button
                    type="button"
                    className="flex items-center gap-1 font-medium"
                    onClick={() => alternarOrdenacao(coluna)}
                  >
                    {coluna.header}
                    {ordenacao?.chave === coluna.key ? (
                      ordenacao.direcao === "asc" ? (
                        <ArrowUp className="size-3.5" />
                      ) : (
                        <ArrowDown className="size-3.5" />
                      )
                    ) : (
                      <ArrowUpDown className="size-3.5 text-muted-foreground/60" />
                    )}
                  </button>
                ) : (
                  coluna.header
                )}
              </TableHead>
            ))}
          </TableRow>
        </TableHeader>
        <TableBody>
          {dadosPagina.map((row) => (
            <TableRow
              key={rowKey(row)}
              className={onRowClick ? "cursor-pointer" : undefined}
              onClick={() => onRowClick?.(row)}
            >
              {columns.map((coluna) => (
                <TableCell key={coluna.key} className={coluna.className}>
                  {coluna.render(row)}
                </TableCell>
              ))}
            </TableRow>
          ))}
        </TableBody>
      </Table>

      {totalPaginas > 1 && (
        <div className="flex items-center justify-between border-t border-border px-4 py-3">
          <p className="text-xs text-muted-foreground">
            Pagina {paginaAtual + 1} de {totalPaginas}
          </p>
          <div className="flex gap-1">
            <Button
              variant="outline"
              size="icon"
              disabled={paginaAtual === 0}
              onClick={() => setPagina((p) => Math.max(0, p - 1))}
            >
              <ChevronLeft className="size-4" />
            </Button>
            <Button
              variant="outline"
              size="icon"
              disabled={paginaAtual >= totalPaginas - 1}
              onClick={() => setPagina((p) => Math.min(totalPaginas - 1, p + 1))}
            >
              <ChevronRight className="size-4" />
            </Button>
          </div>
        </div>
      )}
    </div>
  );
}
