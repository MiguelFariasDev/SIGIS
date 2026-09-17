import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Save } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import type { SchoolStatus } from "@/lib/types/schoolHistory";
import { criarHistoricoEscolar } from "../api";

interface AddSchoolHistoryModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  personId: string;
}

const STATUS_OPCOES: SchoolStatus[] = ["Ativo", "Transferido", "Concluido", "Evadido"];

export function AddSchoolHistoryModal({ open, onOpenChange, personId }: AddSchoolHistoryModalProps) {
  const queryClient = useQueryClient();
  const [schoolName, setSchoolName] = useState("");
  const [grade, setGrade] = useState("");
  const [shift, setShift] = useState("");
  const [classGroup, setClassGroup] = useState("");
  const [schoolYear, setSchoolYear] = useState(new Date().getFullYear().toString());
  const [status, setStatus] = useState<SchoolStatus>("Ativo");
  const [notes, setNotes] = useState("");

  const mutation = useMutation({
    mutationFn: () =>
      criarHistoricoEscolar(personId, {
        schoolName,
        grade,
        shift: shift || undefined,
        classGroup: classGroup || undefined,
        schoolYear: Number(schoolYear),
        status,
        notes: notes || undefined,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["pessoas", personId, "historico-escolar"] });
      toast.success("Historico escolar adicionado.");
      setSchoolName("");
      setGrade("");
      setShift("");
      setClassGroup("");
      setNotes("");
      onOpenChange(false);
    },
    onError: () => toast.error("Nao foi possivel adicionar o historico escolar."),
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Adicionar historico escolar</DialogTitle>
        </DialogHeader>

        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="schoolName">Escola</FieldLabel>
            <Input id="schoolName" value={schoolName} onChange={(e) => setSchoolName(e.target.value)} />
          </Field>
          <div className="grid grid-cols-2 gap-4">
            <Field>
              <FieldLabel htmlFor="grade">Serie/ano</FieldLabel>
              <Input id="grade" value={grade} onChange={(e) => setGrade(e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="schoolYear">Ano letivo</FieldLabel>
              <Input id="schoolYear" type="number" value={schoolYear} onChange={(e) => setSchoolYear(e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="shift">Turno</FieldLabel>
              <Input id="shift" value={shift} onChange={(e) => setShift(e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="classGroup">Turma</FieldLabel>
              <Input id="classGroup" value={classGroup} onChange={(e) => setClassGroup(e.target.value)} />
            </Field>
          </div>
          <Field>
            <FieldLabel htmlFor="status">Situacao</FieldLabel>
            <Select value={status} onValueChange={(v) => setStatus(v as SchoolStatus)}>
              <SelectTrigger id="status" className="w-full">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {STATUS_OPCOES.map((opcao) => (
                  <SelectItem key={opcao} value={opcao}>
                    {opcao}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </Field>
          <Field>
            <FieldLabel htmlFor="notes">Observacoes</FieldLabel>
            <Textarea id="notes" rows={2} value={notes} onChange={(e) => setNotes(e.target.value)} />
          </Field>
        </FieldGroup>

        <DialogFooter>
          <Button
            className="bg-sus-blue hover:bg-sus-blue-dark"
            onClick={() => mutation.mutate()}
            disabled={mutation.isPending || !schoolName || !grade}
          >
            <Save className="size-4" />
            {mutation.isPending ? "Salvando..." : "Adicionar"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
