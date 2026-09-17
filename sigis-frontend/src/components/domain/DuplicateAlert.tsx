import { AlertTriangle } from "lucide-react";
import type { ReactNode } from "react";
import { formatarPercentual } from "@/lib/utils/formatters";

interface DuplicateAlertProps {
  score: number;
  motivo: string;
  children?: ReactNode;
}

export function DuplicateAlert({ score, motivo, children }: DuplicateAlertProps) {
  return (
    <div className="flex gap-3 rounded-xl border border-sus-yellow/40 bg-sus-yellow-light p-4">
      <AlertTriangle className="size-5 shrink-0 text-sus-yellow-dark" />
      <div className="min-w-0 flex-1">
        <p className="text-sm font-semibold text-sus-yellow-dark">
          Possivel duplicata encontrada — {formatarPercentual(score)} similar
        </p>
        <p className="mt-1 text-sm text-sus-yellow-dark/90">{motivo}</p>
        {children && <div className="mt-3">{children}</div>}
      </div>
    </div>
  );
}
