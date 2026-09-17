import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { Save, UserCheck, UsersRound } from "lucide-react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from "@/components/ui/accordion";
import { PageHeader } from "@/components/common/PageHeader";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogFooter, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { ComparacaoLadoALado } from "@/features/duplicidades/components/ComparacaoLadoALado";
import type { DuplicateCandidate } from "@/lib/types/person";
import { ClinicalProfileForm } from "./components/ClinicalProfileForm";
import { ConsentForm } from "./components/ConsentForm";
import { DadosBasicosForm } from "./components/DadosBasicosForm";
import { DevelopmentForm } from "./components/DevelopmentForm";
import { FamilyCompositionForm } from "./components/FamilyCompositionForm";
import { ResponsavelForm } from "./components/ResponsavelForm";
import { SchoolDataForm } from "./components/SchoolDataForm";
import {
  criarPessoa,
  mesclarPessoas,
  registrarConsentimento,
  salvarComposicaoFamiliar,
  salvarDesenvolvimento,
  salvarPerfilClinico,
} from "./api";
import { cadastroPessoaSchema, paraNumero, type CadastroPessoaFormValues } from "./schemas";

const TEM_VALOR = (v: unknown) => v !== undefined && v !== "" && v !== null && v !== false;

export function CadastroPage() {
  const navigate = useNavigate();
  const [candidatoDuplicata, setDuplicateCandidate] = useState<DuplicateCandidate | null>(null);
  const [pessoaCriadaId, setPessoaCriadaId] = useState<string | null>(null);

  const {
    register,
    control,
    handleSubmit,
    getValues,
    formState: { errors },
  } = useForm<CadastroPessoaFormValues>({
    resolver: zodResolver(cadastroPessoaSchema),
    defaultValues: {
      consentClinical: false,
      consentEducational: false,
      consentSocialAssistance: false,
      consentResearch: false,
      referredBySchool: false,
      needsSpecialEducation: false,
      attendsTutoring: false,
      hasFailedGrade: false,
      disabilityTypes: [],
      plannedPregnancy: false,
      locomotionDifficulty: false,
      coordinationDifficulty: false,
      visualDifficulty: false,
      hearingDifficulty: false,
    },
  });

  const criarMutation = useMutation({
    mutationFn: criarPessoa,
    onSuccess: async (pessoa) => {
      const valores = getValues();

      // Cada faceta e um agregado separado no backend — so gravamos se
      // a secao tiver ao menos um campo preenchido (RNF01).
      const tarefas: Promise<unknown>[] = [];

      if (
        [valores.medicalRecordNumber, valores.clinicalHypothesis, valores.apsReferenceUnitId].some(TEM_VALOR)
      ) {
        tarefas.push(
          salvarPerfilClinico(pessoa.id, {
            medicalRecordNumber: valores.medicalRecordNumber,
            clinicalHypothesis: valores.clinicalHypothesis,
            apsReferenceUnitId: valores.apsReferenceUnitId,
          }),
        );
      }

      if ([valores.fatherName, valores.familyMotherName, valores.siblingsCount].some(TEM_VALOR)) {
        tarefas.push(
          salvarComposicaoFamiliar(pessoa.id, {
            fatherName: valores.fatherName,
            fatherEducation: valores.fatherEducation,
            fatherOccupation: valores.fatherOccupation,
            motherName: valores.familyMotherName,
            motherEducation: valores.motherEducation,
            motherOccupation: valores.motherOccupation,
            siblingsCount: paraNumero(valores.siblingsCount),
            siblingsAges: valores.siblingsAges,
            householdMembersCount: paraNumero(valores.householdMembersCount),
            parentsMaritalStatus: valores.parentsMaritalStatus,
            filiationType: valores.filiationType,
            plannedPregnancy: valores.plannedPregnancy,
            pregnanciesCount: paraNumero(valores.pregnanciesCount),
            abortionsCount: paraNumero(valores.abortionsCount),
            pregnancyHealthIssue: valores.pregnancyHealthIssue,
            deliveryType: valores.deliveryType,
            medicationDuringPregnancy: valores.medicationDuringPregnancy,
          }),
        );
      }

      if (
        [valores.ageWalkedMonths, valores.ageTalkedMonths, valores.speechProblems, valores.manualDominance].some(
          TEM_VALOR,
        )
      ) {
        tarefas.push(
          salvarDesenvolvimento(pessoa.id, {
            ageWalkedMonths: paraNumero(valores.ageWalkedMonths),
            ageTalkedMonths: paraNumero(valores.ageTalkedMonths),
            locomotionDifficulty: valores.locomotionDifficulty,
            coordinationDifficulty: valores.coordinationDifficulty,
            visualDifficulty: valores.visualDifficulty,
            hearingDifficulty: valores.hearingDifficulty,
            speechProblems: valores.speechProblems,
            commandComprehension: valores.commandComprehension,
            manualDominance: valores.manualDominance,
          }),
        );
      }

      const consentimentos: { tipo: "Clinical" | "Educational" | "SocialAssistance" | "Research"; granted: boolean }[] = [
        { tipo: "Clinical", granted: valores.consentClinical },
        { tipo: "Educational", granted: valores.consentEducational },
        { tipo: "SocialAssistance", granted: valores.consentSocialAssistance },
        { tipo: "Research", granted: valores.consentResearch },
      ];
      for (const consentimento of consentimentos) {
        tarefas.push(
          registrarConsentimento(pessoa.id, {
            type: consentimento.tipo,
            granted: consentimento.granted,
            version: "1.0",
            evidence: valores.consentCollectedBy
              ? `Coletado por ${valores.consentCollectedBy} em ${valores.consentCollectedAt ?? "data nao informada"}`
              : undefined,
          }),
        );
      }

      await Promise.all(tarefas);

      // O backend real nunca bloqueia a criação: o cadastro já foi
      // persistido acima. Se houver candidatos a duplicidade (RN02), o
      // profissional decide mesclar ou confirmar que são pessoas
      // diferentes — em ambos os casos o cadastro novo já existe.
      setPessoaCriadaId(pessoa.id);
      if (pessoa.candidates.length > 0) {
        setDuplicateCandidate(pessoa.candidates[0]);
        return;
      }

      toast.success("Cadastro realizado com sucesso.");
      navigate(`/pacientes/${pessoa.id}`);
    },
    onError: () => {
      toast.error("Nao foi possivel concluir o cadastro. Tente novamente.");
    },
  });

  const mesclarMutation = useMutation({
    mutationFn: ({ sourceId, targetId }: { sourceId: string; targetId: string }) => mesclarPessoas(sourceId, targetId),
  });

  function montarRequest(values: CadastroPessoaFormValues) {
    return {
      fullName: values.fullName,
      birthDate: values.birthDate,
      cns: values.cns || undefined,
      cpf: values.cpf || undefined,
      motherName: values.motherName || undefined,
      gender: values.gender || undefined,
      raceColor: values.raceColor || undefined,
      phone: values.phone || undefined,
      email: values.email || undefined,
      address:
        values.street || values.number || values.neighborhood || values.city || values.state
          ? {
              street: values.street,
              number: values.number,
              neighborhood: values.neighborhood,
              city: values.city,
              state: values.state,
            }
          : undefined,
      naturality: values.naturality || undefined,
      currentSchool: values.currentSchool || undefined,
      grade: values.grade || undefined,
      shift: values.shift || undefined,
      classGroup: values.classGroup || undefined,
      zone: values.zone,
      schoolEnrollment: values.schoolEnrollment || undefined,
      referredBySchool: values.referredBySchool,
      needsSpecialEducation: values.needsSpecialEducation,
      attendsTutoring: values.attendsTutoring,
      hasFailedGrade: values.hasFailedGrade,
      disabilityTypes: values.disabilityTypes.length > 0 ? values.disabilityTypes.join(", ") : undefined,
      // O backend real não tem endpoint de criação de responsável — este
      // campo do formulário não é enviado (gap documentado).
      guardian: values.guardianName
        ? {
            name: values.guardianName,
            cns: values.guardianCns || undefined,
            birthDate: values.guardianBirthDate || undefined,
            relationship: values.guardianRelationship || "",
          }
        : undefined,
      consentimentoLgpd: values.consentClinical,
    };
  }

  function onSubmit(values: CadastroPessoaFormValues) {
    criarMutation.mutate(montarRequest(values));
  }

  return (
    <div className="mx-auto max-w-3xl">
      <PageHeader title="Novo cadastro" description="Cadastro unico da pessoa na rede de cuidado a TEA." />

      <form onSubmit={handleSubmit(onSubmit)} noValidate>
        <Accordion type="multiple" defaultValue={["dados-pessoais"]} className="space-y-3">
          <AccordionItem value="dados-pessoais" className="rounded-xl border border-border bg-white px-4">
            <AccordionTrigger>1. Dados pessoais</AccordionTrigger>
            <AccordionContent>
              <DadosBasicosForm register={register} control={control} errors={errors} />
            </AccordionContent>
          </AccordionItem>

          <AccordionItem value="dados-escolares" className="rounded-xl border border-border bg-white px-4">
            <AccordionTrigger>2. Dados escolares</AccordionTrigger>
            <AccordionContent>
              <SchoolDataForm register={register} control={control} />
            </AccordionContent>
          </AccordionItem>

          <AccordionItem value="perfil-clinico" className="rounded-xl border border-border bg-white px-4">
            <AccordionTrigger>3. Perfil clinico NASF</AccordionTrigger>
            <AccordionContent>
              <ClinicalProfileForm register={register} control={control} />
            </AccordionContent>
          </AccordionItem>

          <AccordionItem value="composicao-familiar" className="rounded-xl border border-border bg-white px-4">
            <AccordionTrigger>4. Composicao familiar</AccordionTrigger>
            <AccordionContent>
              <FamilyCompositionForm register={register} control={control} />
            </AccordionContent>
          </AccordionItem>

          <AccordionItem value="desenvolvimento" className="rounded-xl border border-border bg-white px-4">
            <AccordionTrigger>5. Desenvolvimento</AccordionTrigger>
            <AccordionContent>
              <DevelopmentForm register={register} control={control} />
            </AccordionContent>
          </AccordionItem>

          <AccordionItem value="consentimentos" className="rounded-xl border border-border bg-white px-4">
            <AccordionTrigger>6. Consentimentos LGPD</AccordionTrigger>
            <AccordionContent>
              <ConsentForm register={register} control={control} errors={errors} />
            </AccordionContent>
          </AccordionItem>

          <AccordionItem value="responsavel" className="rounded-xl border border-border bg-white px-4">
            <AccordionTrigger>7. Responsavel</AccordionTrigger>
            <AccordionContent>
              <ResponsavelForm register={register} errors={errors} />
            </AccordionContent>
          </AccordionItem>
        </Accordion>

        <div className="mt-6 flex justify-end">
          <Button type="submit" className="bg-sus-blue hover:bg-sus-blue-dark" disabled={criarMutation.isPending}>
            <Save className="size-4" />
            {criarMutation.isPending ? "Salvando..." : "Salvar cadastro"}
          </Button>
        </div>
      </form>

      <Dialog open={!!candidatoDuplicata} onOpenChange={(open) => !open && setDuplicateCandidate(null)}>
        <DialogContent className="sm:max-w-2xl">
          <DialogHeader>
            <DialogTitle>Possivel duplicata encontrada</DialogTitle>
          </DialogHeader>

          {candidatoDuplicata && (
            <ComparacaoLadoALado
              tituloA="Cadastro em digitacao"
              tituloB="Cadastro ja existente"
              pessoaA={{
                fullName: getValues("fullName"),
                birthDate: getValues("birthDate"),
                cns: getValues("cns"),
                cpf: getValues("cpf"),
                motherName: getValues("motherName"),
                phone: getValues("phone"),
                street: getValues("street"),
              }}
              pessoaB={candidatoDuplicata}
            />
          )}

          <DialogFooter className="gap-2 sm:justify-between">
            <Button
              variant="outline"
              onClick={() => {
                // O cadastro novo já existe (o backend nunca bloqueia por
                // duplicidade) — só dispensamos o aviso e seguimos para ele.
                setDuplicateCandidate(null);
                if (pessoaCriadaId) {
                  toast.success("Cadastro realizado com sucesso.");
                  navigate(`/pacientes/${pessoaCriadaId}`);
                }
              }}
            >
              <UsersRound className="size-4" />
              Sao pessoas diferentes
            </Button>
            <Button
              className="bg-sus-blue hover:bg-sus-blue-dark"
              disabled={mesclarMutation.isPending}
              onClick={() => {
                if (!pessoaCriadaId || !candidatoDuplicata) return;
                mesclarMutation.mutate(
                  { sourceId: pessoaCriadaId, targetId: candidatoDuplicata.id },
                  {
                    onSuccess: () => {
                      toast.success("Cadastros mesclados com sucesso.");
                      setDuplicateCandidate(null);
                      navigate(`/pacientes/${candidatoDuplicata.id}`);
                    },
                    onError: () => toast.error("Nao foi possivel mesclar os cadastros."),
                  },
                );
              }}
            >
              <UserCheck className="size-4" />
              E a mesma pessoa
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
