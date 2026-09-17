import { differenceInYears, format, formatDistanceToNow, parseISO } from "date-fns";
import { ptBR } from "date-fns/locale";

export function formatarData(iso: string | undefined, pattern = "dd/MM/yyyy"): string {
  if (!iso) return "-";
  try {
    return format(parseISO(iso), pattern, { locale: ptBR });
  } catch {
    return "-";
  }
}

export function formatarDataHora(iso: string | undefined): string {
  return formatarData(iso, "dd/MM/yyyy 'as' HH:mm");
}

export function formatarDistancia(iso: string | undefined): string {
  if (!iso) return "-";
  try {
    return formatDistanceToNow(parseISO(iso), { locale: ptBR, addSuffix: true });
  } catch {
    return "-";
  }
}

export function calcularIdade(dataNascimento: string | undefined): number | null {
  if (!dataNascimento) return null;
  try {
    return differenceInYears(new Date(), parseISO(dataNascimento));
  } catch {
    return null;
  }
}

export function formatarIdade(dataNascimento: string | undefined): string {
  const idade = calcularIdade(dataNascimento);
  if (idade === null) return "-";
  return `${idade} ${idade === 1 ? "ano" : "anos"}`;
}

export function mascararCpf(valor: string): string {
  const digits = valor.replace(/\D/g, "").slice(0, 11);
  return digits
    .replace(/(\d{3})(\d)/, "$1.$2")
    .replace(/(\d{3})(\d)/, "$1.$2")
    .replace(/(\d{3})(\d{1,2})$/, "$1-$2");
}

export function mascararCns(valor: string): string {
  const digits = valor.replace(/\D/g, "").slice(0, 15);
  return digits.replace(/(\d{3})(\d{4})(\d{4})(\d{4})/, "$1 $2 $3 $4");
}

export function mascararTelefone(valor: string): string {
  const digits = valor.replace(/\D/g, "").slice(0, 11);
  if (digits.length <= 10) {
    return digits.replace(/(\d{2})(\d{4})(\d{0,4})/, "($1) $2-$3").trim();
  }
  return digits.replace(/(\d{2})(\d{5})(\d{0,4})/, "($1) $2-$3").trim();
}

export function formatarPercentual(valor: number): string {
  return `${Math.round(valor * 100)}%`;
}
