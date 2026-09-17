import { Home } from "lucide-react";
import { Link } from "react-router-dom";
import { Button } from "@/components/ui/button";

export function NotFoundPage() {
  return (
    <div className="flex min-h-[60vh] flex-col items-center justify-center text-center">
      <p className="text-6xl font-bold text-sus-blue">404</p>
      <p className="mt-2 text-lg font-medium text-foreground">Pagina nao encontrada</p>
      <p className="mt-1 max-w-sm text-sm text-muted-foreground">
        O endereco acessado nao existe ou foi movido.
      </p>
      <Button asChild className="mt-6 bg-sus-blue hover:bg-sus-blue-dark">
        <Link to="/dashboard">
          <Home className="size-4" />
          Voltar ao painel
        </Link>
      </Button>
    </div>
  );
}
