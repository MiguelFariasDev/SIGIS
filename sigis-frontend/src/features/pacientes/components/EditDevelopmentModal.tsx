import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Save } from "lucide-react";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import type { DevelopmentMilestones, ManualDominance } from "@/lib/types/developmentMilestones";
import { salvarDesenvolvimento } from "../api";

interface EditDevelopmentModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  personId: string;
  desenvolvimento: DevelopmentMilestones | null | undefined;
}

const VALOR_INICIAL = {
  ageWalkedMonths: "",
  ageTalkedMonths: "",
  locomotionDifficulty: false,
  coordinationDifficulty: false,
  visualDifficulty: false,
  hearingDifficulty: false,
  speechProblems: "",
  commandComprehension: "",
  communicationForm: "",
  manualDominance: "" as "" | ManualDominance,
};

/** Modal de edicao dos marcos de desenvolvimento (DevelopmentMilestones). */
export function EditDevelopmentModal({ open, onOpenChange, personId, desenvolvimento }: EditDevelopmentModalProps) {
  const queryClient = useQueryClient();
  const [valores, setValores] = useState(VALOR_INICIAL);

  useEffect(() => {
    if (open) {
      setValores({
        ageWalkedMonths: desenvolvimento?.ageWalkedMonths?.toString() ?? "",
        ageTalkedMonths: desenvolvimento?.ageTalkedMonths?.toString() ?? "",
        locomotionDifficulty: desenvolvimento?.locomotionDifficulty ?? false,
        coordinationDifficulty: desenvolvimento?.coordinationDifficulty ?? false,
        visualDifficulty: desenvolvimento?.visualDifficulty ?? false,
        hearingDifficulty: desenvolvimento?.hearingDifficulty ?? false,
        speechProblems: desenvolvimento?.speechProblems ?? "",
        commandComprehension: desenvolvimento?.commandComprehension ?? "",
        communicationForm: desenvolvimento?.communicationForm ?? "",
        manualDominance: desenvolvimento?.manualDominance ?? "",
      });
    }
  }, [open, desenvolvimento]);

  const mutation = useMutation({
    mutationFn: () =>
      salvarDesenvolvimento(personId, {
        ageWalkedMonths: valores.ageWalkedMonths ? Number(valores.ageWalkedMonths) : undefined,
        ageTalkedMonths: valores.ageTalkedMonths ? Number(valores.ageTalkedMonths) : undefined,
        locomotionDifficulty: valores.locomotionDifficulty,
        coordinationDifficulty: valores.coordinationDifficulty,
        visualDifficulty: valores.visualDifficulty,
        hearingDifficulty: valores.hearingDifficulty,
        speechProblems: valores.speechProblems || undefined,
        commandComprehension: valores.commandComprehension || undefined,
        communicationForm: valores.communicationForm || undefined,
        manualDominance: valores.manualDominance || undefined,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["pessoas", personId, "desenvolvimento"] });
      toast.success("Desenvolvimento atualizado.");
      onOpenChange(false);
    },
    onError: () => toast.error("Nao foi possivel salvar os marcos de desenvolvimento."),
  });

  function campo<K extends keyof typeof VALOR_INICIAL>(chave: K, valor: (typeof VALOR_INICIAL)[K]) {
    setValores((atual) => ({ ...atual, [chave]: valor }));
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[85vh] overflow-y-auto sm:max-w-xl">
        <DialogHeader>
          <DialogTitle>Desenvolvimento</DialogTitle>
        </DialogHeader>

        <FieldGroup>
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
            <Field>
              <FieldLabel htmlFor="ageWalkedMonths">Idade que andou (meses)</FieldLabel>
              <Input id="ageWalkedMonths" type="number" value={valores.ageWalkedMonths} onChange={(e) => campo("ageWalkedMonths", e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="ageTalkedMonths">Idade que falou (meses)</FieldLabel>
              <Input id="ageTalkedMonths" type="number" value={valores.ageTalkedMonths} onChange={(e) => campo("ageTalkedMonths", e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="manualDominance">Dominancia manual</FieldLabel>
              <Select value={valores.manualDominance} onValueChange={(v) => campo("manualDominance", v as ManualDominance)}>
                <SelectTrigger id="manualDominance" className="w-full">
                  <SelectValue placeholder="Selecione" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Destro">Destro</SelectItem>
                  <SelectItem value="Canhoto">Canhoto</SelectItem>
                  <SelectItem value="Ambidestro">Ambidestro</SelectItem>
                </SelectContent>
              </Select>
            </Field>
          </div>

          <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
            {(
              [
                ["locomotionDifficulty", "Dificuldade de locomocao"],
                ["coordinationDifficulty", "Dificuldade de coordenacao"],
                ["visualDifficulty", "Dificuldade visual"],
                ["hearingDifficulty", "Dificuldade auditiva"],
              ] as const
            ).map(([chave, label]) => (
              <label key={chave} className="flex items-center gap-2 text-sm text-foreground">
                <Checkbox checked={valores[chave]} onCheckedChange={(v) => campo(chave, v === true)} />
                {label}
              </label>
            ))}
          </div>

          <Field>
            <FieldLabel htmlFor="speechProblems">Problemas de fala</FieldLabel>
            <Textarea id="speechProblems" rows={2} value={valores.speechProblems} onChange={(e) => campo("speechProblems", e.target.value)} />
          </Field>
          <Field>
            <FieldLabel htmlFor="commandComprehension">Compreensao de comandos</FieldLabel>
            <Textarea id="commandComprehension" rows={2} value={valores.commandComprehension} onChange={(e) => campo("commandComprehension", e.target.value)} />
          </Field>
          <Field>
            <FieldLabel htmlFor="communicationForm">Forma de comunicacao</FieldLabel>
            <Textarea id="communicationForm" rows={2} value={valores.communicationForm} onChange={(e) => campo("communicationForm", e.target.value)} />
          </Field>
        </FieldGroup>

        <DialogFooter>
          <Button className="bg-sus-blue hover:bg-sus-blue-dark" onClick={() => mutation.mutate()} disabled={mutation.isPending}>
            <Save className="size-4" />
            {mutation.isPending ? "Salvando..." : "Salvar"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
