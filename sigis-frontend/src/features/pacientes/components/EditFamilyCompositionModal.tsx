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
import type { FamilyComposition } from "@/lib/types/familyComposition";
import { salvarComposicaoFamiliar } from "../api";

interface EditFamilyCompositionModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  personId: string;
  composicao: FamilyComposition | null | undefined;
}

const VALOR_INICIAL = {
  fatherName: "",
  fatherEducation: "",
  fatherOccupation: "",
  motherName: "",
  motherEducation: "",
  motherOccupation: "",
  siblingsCount: "",
  siblingsAges: "",
  householdMembersCount: "",
  parentsMaritalStatus: "",
  filiationType: "" as "" | "Natural" | "Adotivo",
  plannedPregnancy: false,
  pregnanciesCount: "",
  abortionsCount: "",
  pregnancyHealthIssue: "",
  deliveryType: "" as "" | "Normal" | "Cesarea",
  medicationDuringPregnancy: "",
};

/** Modal de edicao da composicao familiar — Anexo A (RF02 faceta Educacao/NASF). */
export function EditFamilyCompositionModal({ open, onOpenChange, personId, composicao }: EditFamilyCompositionModalProps) {
  const queryClient = useQueryClient();
  const [valores, setValores] = useState(VALOR_INICIAL);

  useEffect(() => {
    if (open) {
      setValores({
        fatherName: composicao?.fatherName ?? "",
        fatherEducation: composicao?.fatherEducation ?? "",
        fatherOccupation: composicao?.fatherOccupation ?? "",
        motherName: composicao?.motherName ?? "",
        motherEducation: composicao?.motherEducation ?? "",
        motherOccupation: composicao?.motherOccupation ?? "",
        siblingsCount: composicao?.siblingsCount?.toString() ?? "",
        siblingsAges: composicao?.siblingsAges ?? "",
        householdMembersCount: composicao?.householdMembersCount?.toString() ?? "",
        parentsMaritalStatus: composicao?.parentsMaritalStatus ?? "",
        filiationType: composicao?.filiationType ?? "",
        plannedPregnancy: composicao?.plannedPregnancy ?? false,
        pregnanciesCount: composicao?.pregnanciesCount?.toString() ?? "",
        abortionsCount: composicao?.abortionsCount?.toString() ?? "",
        pregnancyHealthIssue: composicao?.pregnancyHealthIssue ?? "",
        deliveryType: composicao?.deliveryType ?? "",
        medicationDuringPregnancy: composicao?.medicationDuringPregnancy ?? "",
      });
    }
  }, [open, composicao]);

  const mutation = useMutation({
    mutationFn: () =>
      salvarComposicaoFamiliar(personId, {
        fatherName: valores.fatherName || undefined,
        fatherEducation: valores.fatherEducation || undefined,
        fatherOccupation: valores.fatherOccupation || undefined,
        motherName: valores.motherName || undefined,
        motherEducation: valores.motherEducation || undefined,
        motherOccupation: valores.motherOccupation || undefined,
        siblingsCount: valores.siblingsCount ? Number(valores.siblingsCount) : undefined,
        siblingsAges: valores.siblingsAges || undefined,
        householdMembersCount: valores.householdMembersCount ? Number(valores.householdMembersCount) : undefined,
        parentsMaritalStatus: valores.parentsMaritalStatus || undefined,
        filiationType: valores.filiationType || undefined,
        plannedPregnancy: valores.plannedPregnancy,
        pregnanciesCount: valores.pregnanciesCount ? Number(valores.pregnanciesCount) : undefined,
        abortionsCount: valores.abortionsCount ? Number(valores.abortionsCount) : undefined,
        pregnancyHealthIssue: valores.pregnancyHealthIssue || undefined,
        deliveryType: valores.deliveryType || undefined,
        medicationDuringPregnancy: valores.medicationDuringPregnancy || undefined,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["pessoas", personId, "composicao-familiar"] });
      toast.success("Composicao familiar atualizada.");
      onOpenChange(false);
    },
    onError: () => toast.error("Nao foi possivel salvar a composicao familiar."),
  });

  function campo<K extends keyof typeof VALOR_INICIAL>(chave: K, valor: (typeof VALOR_INICIAL)[K]) {
    setValores((atual) => ({ ...atual, [chave]: valor }));
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-h-[85vh] overflow-y-auto sm:max-w-xl">
        <DialogHeader>
          <DialogTitle>Composicao familiar</DialogTitle>
        </DialogHeader>

        <FieldGroup>
          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Field>
              <FieldLabel htmlFor="fatherName">Nome do pai</FieldLabel>
              <Input id="fatherName" value={valores.fatherName} onChange={(e) => campo("fatherName", e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="motherName">Nome da mae</FieldLabel>
              <Input id="motherName" value={valores.motherName} onChange={(e) => campo("motherName", e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="fatherEducation">Escolaridade do pai</FieldLabel>
              <Input id="fatherEducation" value={valores.fatherEducation} onChange={(e) => campo("fatherEducation", e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="motherEducation">Escolaridade da mae</FieldLabel>
              <Input id="motherEducation" value={valores.motherEducation} onChange={(e) => campo("motherEducation", e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="fatherOccupation">Ocupacao do pai</FieldLabel>
              <Input id="fatherOccupation" value={valores.fatherOccupation} onChange={(e) => campo("fatherOccupation", e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="motherOccupation">Ocupacao da mae</FieldLabel>
              <Input id="motherOccupation" value={valores.motherOccupation} onChange={(e) => campo("motherOccupation", e.target.value)} />
            </Field>
          </div>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
            <Field>
              <FieldLabel htmlFor="siblingsCount">Numero de irmaos</FieldLabel>
              <Input id="siblingsCount" type="number" value={valores.siblingsCount} onChange={(e) => campo("siblingsCount", e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="siblingsAges">Idades dos irmaos</FieldLabel>
              <Input id="siblingsAges" value={valores.siblingsAges} onChange={(e) => campo("siblingsAges", e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="householdMembersCount">Pessoas no domicilio</FieldLabel>
              <Input id="householdMembersCount" type="number" value={valores.householdMembersCount} onChange={(e) => campo("householdMembersCount", e.target.value)} />
            </Field>
          </div>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Field>
              <FieldLabel htmlFor="parentsMaritalStatus">Estado civil dos pais</FieldLabel>
              <Input id="parentsMaritalStatus" value={valores.parentsMaritalStatus} onChange={(e) => campo("parentsMaritalStatus", e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="filiationType">Tipo de filiacao</FieldLabel>
              <Select value={valores.filiationType} onValueChange={(v) => campo("filiationType", v as "Natural" | "Adotivo")}>
                <SelectTrigger id="filiationType" className="w-full">
                  <SelectValue placeholder="Selecione" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Natural">Natural</SelectItem>
                  <SelectItem value="Adotivo">Adotivo</SelectItem>
                </SelectContent>
              </Select>
            </Field>
          </div>

          <p className="text-sm font-semibold text-foreground">Gestacao</p>

          <label className="flex items-center gap-2 text-sm text-foreground">
            <Checkbox checked={valores.plannedPregnancy} onCheckedChange={(v) => campo("plannedPregnancy", v === true)} />
            Gravidez planejada
          </label>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-3">
            <Field>
              <FieldLabel htmlFor="pregnanciesCount">Numero de gestacoes</FieldLabel>
              <Input id="pregnanciesCount" type="number" value={valores.pregnanciesCount} onChange={(e) => campo("pregnanciesCount", e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="abortionsCount">Numero de abortos</FieldLabel>
              <Input id="abortionsCount" type="number" value={valores.abortionsCount} onChange={(e) => campo("abortionsCount", e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="deliveryType">Tipo de parto</FieldLabel>
              <Select value={valores.deliveryType} onValueChange={(v) => campo("deliveryType", v as "Normal" | "Cesarea")}>
                <SelectTrigger id="deliveryType" className="w-full">
                  <SelectValue placeholder="Selecione" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Normal">Normal</SelectItem>
                  <SelectItem value="Cesarea">Cesarea</SelectItem>
                </SelectContent>
              </Select>
            </Field>
          </div>

          <Field>
            <FieldLabel htmlFor="pregnancyHealthIssue">Intercorrencias na gestacao</FieldLabel>
            <Textarea id="pregnancyHealthIssue" rows={2} value={valores.pregnancyHealthIssue} onChange={(e) => campo("pregnancyHealthIssue", e.target.value)} />
          </Field>

          <Field>
            <FieldLabel htmlFor="medicationDuringPregnancy">Medicacao durante a gestacao</FieldLabel>
            <Textarea id="medicationDuringPregnancy" rows={2} value={valores.medicationDuringPregnancy} onChange={(e) => campo("medicationDuringPregnancy", e.target.value)} />
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
