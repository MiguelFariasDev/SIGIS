import { Skeleton } from "@/components/ui/skeleton";

interface LoadingStateProps {
  rows?: number;
  label?: string;
}

export function LoadingState({ rows = 4, label = "Carregando..." }: LoadingStateProps) {
  return (
    <div className="space-y-3 py-4" role="status" aria-label={label}>
      {Array.from({ length: rows }).map((_, index) => (
        <Skeleton key={index} className="h-12 w-full rounded-lg" />
      ))}
    </div>
  );
}
