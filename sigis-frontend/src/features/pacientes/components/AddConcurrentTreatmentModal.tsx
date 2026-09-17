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
import type { DayOfWeek } from "@/lib/types/concurrentTreatment";
import { criarTratamentoConcomitante } from "../api";

interface AddConcurrentTreatmentModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  personId: string;
}

const DIAS: { value: DayOfWeek; label: string }[] = [
  { value: "Monday", label: "Segunda-feira" },
  { value: "Tuesday", label: "Terca-feira" },
  { value: "Wednesday", label: "Quarta-feira" },
  { value: "Thursday", label: "Quinta-feira" },
  { value: "Friday", label: "Sexta-feira" },
  { value: "Saturday", label: "Sabado" },
  { value: "Sunday", label: "Domingo" },
];

export function AddConcurrentTreatmentModal({ open, onOpenChange, personId }: AddConcurrentTreatmentModalProps) {
  const queryClient = useQueryClient();
  const [specialty, setSpecialty] = useState("");
  const [location, setLocation] = useState("");
  const [professionalName, setProfessionalName] = useState("");
  const [dayOfWeek, setDayOfWeek] = useState<DayOfWeek>("Monday");
  const [startTime, setStartTime] = useState("");
  const [endTime, setEndTime] = useState("");
  const [notes, setNotes] = useState("");

  const mutation = useMutation({
    mutationFn: () =>
      criarTratamentoConcomitante(personId, {
        specialty,
        location,
        professionalName,
        dayOfWeek,
        startTime,
        endTime,
        notes: notes || undefined,
      }),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["pessoas", personId, "tratamentos-concomitantes"] });
      toast.success("Tratamento concomitante adicionado.");
      setSpecialty("");
      setLocation("");
      setProfessionalName("");
      setStartTime("");
      setEndTime("");
      setNotes("");
      onOpenChange(false);
    },
    onError: () => toast.error("Nao foi possivel adicionar o tratamento concomitante."),
  });

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Adicionar tratamento concomitante</DialogTitle>
        </DialogHeader>

        <FieldGroup>
          <div className="grid grid-cols-2 gap-4">
            <Field>
              <FieldLabel htmlFor="specialty">Especialidade</FieldLabel>
              <Input id="specialty" value={specialty} onChange={(e) => setSpecialty(e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="location">Local</FieldLabel>
              <Input id="location" value={location} onChange={(e) => setLocation(e.target.value)} />
            </Field>
          </div>
          <Field>
            <FieldLabel htmlFor="professionalName">Profissional</FieldLabel>
            <Input id="professionalName" value={professionalName} onChange={(e) => setProfessionalName(e.target.value)} />
          </Field>
          <div className="grid grid-cols-3 gap-4">
            <Field>
              <FieldLabel htmlFor="dayOfWeek">Dia da semana</FieldLabel>
              <Select value={dayOfWeek} onValueChange={(v) => setDayOfWeek(v as DayOfWeek)}>
                <SelectTrigger id="dayOfWeek" className="w-full">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {DIAS.map((dia) => (
                    <SelectItem key={dia.value} value={dia.value}>
                      {dia.label}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </Field>
            <Field>
              <FieldLabel htmlFor="startTime">Inicio</FieldLabel>
              <Input id="startTime" type="time" value={startTime} onChange={(e) => setStartTime(e.target.value)} />
            </Field>
            <Field>
              <FieldLabel htmlFor="endTime">Fim</FieldLabel>
              <Input id="endTime" type="time" value={endTime} onChange={(e) => setEndTime(e.target.value)} />
            </Field>
          </div>
          <Field>
            <FieldLabel htmlFor="notes">Observacoes</FieldLabel>
            <Textarea id="notes" rows={2} value={notes} onChange={(e) => setNotes(e.target.value)} />
          </Field>
        </FieldGroup>

        <DialogFooter>
          <Button
            className="bg-sus-blue hover:bg-sus-blue-dark"
            onClick={() => mutation.mutate()}
            disabled={mutation.isPending || !specialty || !professionalName}
          >
            <Save className="size-4" />
            {mutation.isPending ? "Salvando..." : "Adicionar"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
