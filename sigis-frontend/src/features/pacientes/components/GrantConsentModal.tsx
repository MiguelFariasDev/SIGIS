import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Save } from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Field, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import type { ConsentType } from "@/lib/types/personConsent";
import { registrarConsentimento } from "../api";

interface GrantConsentModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  personId: string;
}

const TIPOS: { value: ConsentType; label: string }[] = [
  { value: "Clinical", label: "Clinico (Saude)" },
  { value: "Educational", label: "Educacional" },
  { value: "SocialAssistance", label: "Assistencia Social" },
  { value: "Research", label: "Pesquisa" },
];

export function GrantConsentModal({ open, onOpenChange, personId }: GrantConsentModalProps) {
  const queryClient = useQueryClient();
  const [type, setType] = useState<ConsentType>("Clinical");
  const [evidence, setEvidence] = useState("");

  const mutation = useMutation({
    mutationFn: () =>
      registrarConsentimento(personId, {
        type,
        granted: true,
        version: "1.0",
        evidence: evidence || undefined,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["pessoas", personId, "consentimentos"] });
      toast.success("Consentimento registrado.");
      setEvidence("");
      onOpenChange(false);
    },
    onError: () => toast.error("Nao foi possivel registrar o consentimento."),
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Registrar consentimento</DialogTitle>
        </DialogHeader>

        <FieldGroup>
          <Field>
            <FieldLabel htmlFor="type">Tipo</FieldLabel>
            <Select value={type} onValueChange={(v) => setType(v as ConsentType)}>
              <SelectTrigger id="type" className="w-full">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                {TIPOS.map((tipo) => (
                  <SelectItem key={tipo.value} value={tipo.value}>
                    {tipo.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </Field>
          <Field>
            <FieldLabel htmlFor="evidence">Evidencia da coleta (opcional)</FieldLabel>
            <Textarea
              id="evidence"
              rows={3}
              placeholder="Ex.: Coletado por telefone junto ao responsavel em..."
              value={evidence}
              onChange={(e) => setEvidence(e.target.value)}
            />
          </Field>
        </FieldGroup>

        <DialogFooter>
          <Button className="bg-sus-blue hover:bg-sus-blue-dark" onClick={() => mutation.mutate()} disabled={mutation.isPending}>
            <Save className="size-4" />
            {mutation.isPending ? "Salvando..." : "Registrar"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
