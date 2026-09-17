import type { LucideIcon } from "lucide-react";
import { cn } from "@/lib/utils";

interface IndicatorCardProps {
  icon: LucideIcon;
  label: string;
  value: number | string;
  tone?: "neutral" | "danger" | "success";
  loading?: boolean;
}

const TONE_STYLES: Record<NonNullable<IndicatorCardProps["tone"]>, string> = {
  neutral: "bg-sus-blue-light text-sus-blue",
  danger: "bg-sus-red-light text-sus-red",
  success: "bg-sus-green-light text-sus-green",
};

export function IndicatorCard({ icon: Icon, label, value, tone = "neutral", loading = false }: IndicatorCardProps) {
  return (
    <div className="rounded-xl border border-border bg-white p-5">
      <div className="flex items-center justify-between">
        <p className="text-sm font-medium text-muted-foreground">{label}</p>
        <div className={cn("flex size-9 items-center justify-center rounded-full", TONE_STYLES[tone])}>
          <Icon className="size-4.5" />
        </div>
      </div>
      <p className="mt-3 text-3xl font-bold tracking-tight text-foreground">{loading ? "—" : value}</p>
    </div>
  );
}
