import { CartesianGrid, Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import type { AtendimentosPorDiaItem } from "@/lib/types/indicadores";
import { formatarData } from "@/lib/utils/formatters";

interface AtendimentosChartProps {
  data: AtendimentosPorDiaItem[];
}

export function AtendimentosChart({ data }: AtendimentosChartProps) {
  const formatado = data.map((item) => ({ ...item, dataLabel: formatarData(item.data, "dd/MM") }));

  return (
    <ResponsiveContainer width="100%" height={280}>
      <LineChart data={formatado}>
        <CartesianGrid vertical={false} stroke="var(--border)" />
        <XAxis dataKey="dataLabel" tickLine={false} axisLine={false} fontSize={12} stroke="var(--muted-foreground)" />
        <YAxis allowDecimals={false} tickLine={false} axisLine={false} fontSize={12} stroke="var(--muted-foreground)" />
        <Tooltip
          contentStyle={{ borderRadius: 8, borderColor: "var(--border)", fontSize: 13 }}
          formatter={(value) => [value, "Atendimentos"]}
        />
        <Line
          type="monotone"
          dataKey="quantidade"
          name="Atendimentos"
          stroke="var(--color-sus-blue)"
          strokeWidth={2}
          dot={{ r: 3, fill: "var(--color-sus-blue)" }}
          activeDot={{ r: 5 }}
        />
      </LineChart>
    </ResponsiveContainer>
  );
}
