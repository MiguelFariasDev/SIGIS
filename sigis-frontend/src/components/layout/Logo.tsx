import { cn } from "@/lib/utils";

/**
 * PLACEHOLDER DE MARCA — SUBSTITUIR ANTES DE QUALQUER USO PUBLICO/OFICIAL.
 *
 * O simbolo oficial do SUS e protegido por legislacao federal e NAO deve
 * ser usado aqui. Este icone (escudo + coracao + rede de pontos) e uma
 * criacao propria do SIGIS, apenas inspirada na paleta de saude publica
 * (azul/verde). Quando a Prefeitura fornecer identidade visual oficial,
 * troque o SVG abaixo mantendo a mesma API do componente.
 */

interface LogoProps {
  className?: string;
  iconOnly?: boolean;
  size?: number;
  variant?: "light" | "dark";
}

export function Logo({ className, iconOnly = false, size = 40, variant = "light" }: LogoProps) {
  return (
    <div className={cn("flex items-center gap-3", className)}>
      <svg
        width={size}
        height={size}
        viewBox="0 0 40 40"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
        role="img"
        aria-label="Logo SIGIS"
      >
        <rect width="40" height="40" rx="10" fill="#0056A6" />
        <path
          d="M20 27.5c-.35 0-.68-.12-.95-.34-1.44-1.17-2.83-2.27-4.03-3.32-3.52-3.07-5.52-5.5-5.52-8.4 0-2.42 1.9-4.32 4.32-4.32 1.37 0 2.68.64 3.53 1.65a4.63 4.63 0 0 1 .65 1.02.29.29 0 0 0 .52 0c.17-.36.38-.71.65-1.02a4.7 4.7 0 0 1 3.53-1.65c2.42 0 4.32 1.9 4.32 4.32 0 2.9-2 5.33-5.52 8.4-1.2 1.05-2.59 2.15-4.03 3.32-.27.22-.6.34-.95.34Z"
          fill="white"
        />
        <circle cx="11" cy="30.5" r="2" fill="#FFCB00" />
        <circle cx="20" cy="32.5" r="2" fill="#00A859" />
        <circle cx="29" cy="30.5" r="2" fill="#FFCB00" />
        <path
          d="M11 30.5 20 32.5 29 30.5"
          stroke="white"
          strokeOpacity="0.85"
          strokeWidth="1.4"
          strokeLinecap="round"
        />
      </svg>
      {!iconOnly && (
        <div className="leading-tight">
          <p
            className={cn(
              "text-lg font-bold tracking-tight",
              variant === "light" ? "text-white" : "text-sus-blue-dark",
            )}
          >
            SIGIS
          </p>
          <p className={cn("text-[11px]", variant === "light" ? "text-white/80" : "text-neutral-500")}>
            Sistema Integrado de Gestao e Informacao em Saude
          </p>
        </div>
      )}
    </div>
  );
}
