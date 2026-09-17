import { ConfirmDialog } from "@/components/common/ConfirmDialog";

interface MergeConfirmDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onConfirm: () => void;
  nomeA: string;
  nomeB: string;
}

export function MergeConfirmDialog({ open, onOpenChange, onConfirm, nomeA, nomeB }: MergeConfirmDialogProps) {
  return (
    <ConfirmDialog
      open={open}
      onOpenChange={onOpenChange}
      title="Confirmar mesclagem de cadastros"
      description={`"${nomeA}" e "${nomeB}" serao tratados como a mesma pessoa. O historico de atendimentos, filas e encaminhamentos de ambos os cadastros sera preservado sob um identificador unico. Essa acao nao pode ser desfeita pela interface.`}
      confirmLabel="Confirmar mesma pessoa"
      onConfirm={onConfirm}
    />
  );
}
