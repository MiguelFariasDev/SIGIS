import { Bar, BarChart, CartesianGrid, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis } from "recharts";
import type { FilaPorServicoItem } from "@/lib/types/indicadores";
import { SERVICO_LABEL, type ServicoSigla } from "@/lib/utils/constants";

interface FilaPorServicoChartProps {
  data: FilaPorServicoItem[];
}

export function FilaPorServicoChart({ data }: FilaPorServicoChartProps) {
  const formatado = data.map((item) => ({
    ...item,
    servicoLabel: SERVICO_LABEL[item.servico as ServicoSigla] ?? item.servico,
  }));

  return (
    <ResponsiveContainer width="100%" height={280}>
      <BarChart data={formatado} barCategoryGap={24} barGap={4}>
        <CartesianGrid vertical={false} stroke="var(--border)" />
        <XAxis dataKey="servicoLabel" tickLine={false} axisLine={false} fontSize={12} stroke="var(--muted-foreground)" />
        <YAxis allowDecimals={false} tickLine={false} axisLine={false} fontSize={12} stroke="var(--muted-foreground)" />
        <Tooltip
          contentStyle={{ borderRadius: 8, borderColor: "var(--border)", fontSize: 13 }}
          cursor={{ fill: "var(--muted)" }}
        />
        <Legend wrapperStyle={{ fontSize: 12 }} />
        <Bar dataKey="aguardando" name="Aguardando" fill="var(--color-sus-blue)" radius={[4, 4, 0, 0]} maxBarSize={28} />
        <Bar
          dataKey="emAtendimento"
          name="Em atendimento"
          fill="var(--color-sus-green)"
          radius={[4, 4, 0, 0]}
          maxBarSize={28}
        />
      </BarChart>
    </ResponsiveContainer>
  );
}
