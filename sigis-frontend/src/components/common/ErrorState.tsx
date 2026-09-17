import { AlertCircle, RefreshCw } from "lucide-react";
import { Button } from "@/components/ui/button";

interface ErrorStateProps {
  title?: string;
  description?: string;
  onRetry?: () => void;
}

export function ErrorState({
  title = "Nao foi possivel carregar os dados",
  description = "Verifique sua conexao e tente novamente.",
  onRetry,
}: ErrorStateProps) {
  return (
    <div className="flex flex-col items-center justify-center rounded-xl border border-sus-red/20 bg-sus-red-light px-6 py-16 text-center">
      <div className="mb-4 flex size-12 items-center justify-center rounded-full bg-white text-sus-red">
        <AlertCircle className="size-6" />
      </div>
      <p className="text-base font-medium text-sus-red-dark">{title}</p>
      <p className="mt-1 max-w-sm text-sm text-sus-red-dark/80">{description}</p>
      {onRetry && (
        <Button variant="outline" className="mt-4" onClick={onRetry}>
          <RefreshCw className="size-4" />
          Tentar novamente
        </Button>
      )}
    </div>
  );
}
