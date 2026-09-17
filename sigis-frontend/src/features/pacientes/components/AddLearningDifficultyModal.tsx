import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Save } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import type { LearningDifficultySeverity, LearningDifficultyType } from "@/lib/types/learningDifficulties";
import { criarDificuldadeAprendizagem } from "../api";

interface AddLearningDifficultyModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  personId: string;
}

const TIPOS: LearningDifficultyType[] = [
  "Leitura",
  "Escrita",
  "Calculo",
  "Atencao",
  "Concentracao",
  "CoordenacaoMotora",
  "Outro",
];

const SEVERIDADES: LearningDifficultySeverity[] = ["Leve", "Moderada", "Severa"];

export function AddLearningDifficultyModal({ open, onOpenChange, personId }: AddLearningDifficultyModalProps) {
  const queryClient = useQueryClient();
  const [type, setType] = useState<LearningDifficultyType>("Leitura");
  const [severity, setSeverity] = useState<LearningDifficultySeverity>("Leve");
  const [notes, setNotes] = useState("");

  const mutation = useMutation({
    mutationFn: () =>
      criarDificuldadeAprendizagem(personId, {
        type,
        severity,
        assessmentDate: new Date().toISOString().slice(0, 10),
        notes: notes || undefined,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["pessoas", personId, "dificuldades-aprendizagem"] });
      toast.success("Dificuldade de aprendizagem registrada.");
      setNotes("");
      onOpenChange(false);
    },
    onError: () => toast.error("Nao foi possivel registrar a dificuldade de aprendizagem."),
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Adicionar dificuldade de aprendizagem</DialogTitle>
        </DialogHeader>

        <FieldGroup>
          <div className="grid grid-cols-2 gap-4">
            <Field>
              <FieldLabel htmlFor="type">Tipo</FieldLabel>
              <Select value={type} onValueChange={(v) => setType(v as LearningDifficultyType)}>
                <SelectTrigger id="type" className="w-full">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {TIPOS.map((opcao) => (
                    <SelectItem key={opcao} value={opcao}>
                      {opcao}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </Field>
            <Field>
              <FieldLabel htmlFor="severity">Severidade</FieldLabel>
              <Select value={severity} onValueChange={(v) => setSeverity(v as LearningDifficultySeverity)}>
                <SelectTrigger id="severity" className="w-full">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {SEVERIDADES.map((opcao) => (
                    <SelectItem key={opcao} value={opcao}>
                      {opcao}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </Field>
          </div>
          <Field>
            <FieldLabel htmlFor="notes">Observacoes</FieldLabel>
            <Textarea id="notes" rows={3} value={notes} onChange={(e) => setNotes(e.target.value)} />
          </Field>
        </FieldGroup>

        <DialogFooter>
          <Button className="bg-sus-blue hover:bg-sus-blue-dark" onClick={() => mutation.mutate()} disabled={mutation.isPending}>
            <Save className="size-4" />
            {mutation.isPending ? "Salvando..." : "Adicionar"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
