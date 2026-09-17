import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Save } from "lucide-react";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { unidadesMock } from "@/lib/mocks/data/unidades.mock";
import { SecretariaResponsavel } from "@/lib/types/enums";
import type { PersonClinicalProfile } from "@/lib/types/personClinicalProfile";
import { salvarPerfilClinico } from "../api";

const UNIDADES_SAUDE = unidadesMock.filter((u) => u.secretariaResponsavel === SecretariaResponsavel.SAUDE);

interface EditClinicalProfileModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  personId: string;
  perfil: PersonClinicalProfile | null | undefined;
}

/** Modal de edicao do perfil clinico NASF (PersonClinicalProfile) — agregado separado da Pessoa. */
export function EditClinicalProfileModal({ open, onOpenChange, personId, perfil }: EditClinicalProfileModalProps) {
  const queryClient = useQueryClient();
  const [medicalRecordNumber, setMedicalRecordNumber] = useState("");
  const [clinicalHypothesis, setClinicalHypothesis] = useState("");
  const [apsReferenceUnitId, setApsReferenceUnitId] = useState("");

  useEffect(() => {
    if (open) {
      setMedicalRecordNumber(perfil?.medicalRecordNumber ?? "");
      setClinicalHypothesis(perfil?.clinicalHypothesis ?? "");
      setApsReferenceUnitId(perfil?.apsReferenceUnitId ?? "");
    }
  }, [open, perfil]);

  const mutation = useMutation({
    mutationFn: () =>
      salvarPerfilClinico(personId, {
        medicalRecordNumber: medicalRecordNumber || undefined,
        clinicalHypothesis: clinicalHypothesis || undefined,
        apsReferenceUnitId: apsReferenceUnitId || undefined,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["pessoas", personId, "perfil-clinico"] });
      toast.success("Perfil clinico atualizado.");
      onOpenChange(false);
    },
    onError: () => toast.error("Nao foi possivel salvar o perfil clinico."),
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Perfil clinico NASF</DialogTitle>
        </DialogHeader>

        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="medicalRecordNumber">Numero do prontuario</FieldLabel>
            <Input
              id="medicalRecordNumber"
              value={medicalRecordNumber}
              onChange={(e) => setMedicalRecordNumber(e.target.value)}
            />
          </Field>
          <Field>
            <FieldLabel htmlFor="clinicalHypothesis">Hipotese clinica</FieldLabel>
            <Textarea
              id="clinicalHypothesis"
              rows={3}
              value={clinicalHypothesis}
              onChange={(e) => setClinicalHypothesis(e.target.value)}
            />
          </Field>
          <Field>
            <FieldLabel htmlFor="apsReferenceUnitId">Unidade de referencia (APS)</FieldLabel>
            <Select value={apsReferenceUnitId} onValueChange={setApsReferenceUnitId}>
              <SelectTrigger id="apsReferenceUnitId" className="w-full">
                <SelectValue placeholder="Selecione a unidade" />
              </SelectTrigger>
              <SelectContent>
                {UNIDADES_SAUDE.map((u) => (
                  <SelectItem key={u.id} value={u.id}>
                    {u.nome} ({u.sigla})
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </Field>
        </FieldGroup>

        <DialogFooter>
          <Button
            className="bg-sus-blue hover:bg-sus-blue-dark"
            onClick={() => mutation.mutate()}
            disabled={mutation.isPending}
          >
            <Save className="size-4" />
            {mutation.isPending ? "Salvando..." : "Salvar"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
