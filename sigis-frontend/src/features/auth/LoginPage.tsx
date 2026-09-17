import { zodResolver } from "@hookform/resolvers/zod";
import { LogIn } from "lucide-react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { z } from "zod";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Field, FieldError, FieldGroup, FieldLabel } from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { Logo } from "@/components/layout/Logo";
import { PapelRbac } from "@/lib/types/enums";
import { useLogin } from "./useAuth";

const loginSchema = z.object({
  email: z.string().min(1, "Informe o e-mail ou matricula.").email("E-mail invalido."),
  senha: z.string().min(1, "Informe a senha."),
  lembrarMe: z.boolean(),
});

type LoginFormValues = z.infer<typeof loginSchema>;

const CONTAS_DEMO = [
  { papel: "Profissional", email: "camila.teixeira@crateus.ce.gov.br" },
  { papel: "Coordenador(a)", email: "roberto.meneses@crateus.ce.gov.br" },
  { papel: "Auditor(a) / DPO", email: "fernanda.cavalcante@crateus.ce.gov.br" },
];

export function LoginPage() {
  const navigate = useNavigate();
  const loginMutation = useLogin();

  const {
    register,
    handleSubmit,
    setValue,
    watch,
    formState: { errors },
  } = useForm<LoginFormValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: { email: "", senha: "", lembrarMe: false },
  });

  function onSubmit(values: LoginFormValues) {
    loginMutation.mutate(
      { email: values.email, senha: values.senha, lembrarMe: values.lembrarMe },
      {
        onSuccess: (data) => {
          toast.success(`Bem-vindo(a), ${data.profissional.nome.split(" ")[0]}!`);
          navigate(data.profissional.papelRbac === PapelRbac.AUDITOR ? "/auditoria" : "/dashboard", {
            replace: true,
          });
        },
        onError: () => {
          toast.error("Credenciais invalidas. Verifique o e-mail e a senha.");
        },
      },
    );
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-gradient-to-br from-sus-blue via-sus-blue to-white p-4">
      <div className="w-full max-w-md rounded-2xl bg-white p-8 shadow-xl">
        <div className="mb-8 flex flex-col items-center text-center">
          <Logo variant="dark" size={56} iconOnly />
          <h1 className="mt-4 text-2xl font-bold text-sus-blue-dark">SIGIS</h1>
          <p className="mt-1 text-sm text-neutral-500">Sistema Integrado de Gestao e Informacao em Saude</p>
        </div>

        <form onSubmit={handleSubmit(onSubmit)} noValidate>
          <FieldGroup>
            <Field data-invalid={!!errors.email}>
              <FieldLabel htmlFor="email">E-mail ou matricula</FieldLabel>
              <Input id="email" type="email" autoComplete="username" placeholder="seuemail@crateus.ce.gov.br" {...register("email")} />
              <FieldError errors={errors.email ? [errors.email] : undefined} />
            </Field>

            <Field data-invalid={!!errors.senha}>
              <FieldLabel htmlFor="senha">Senha</FieldLabel>
              <Input id="senha" type="password" autoComplete="current-password" placeholder="********" {...register("senha")} />
              <FieldError errors={errors.senha ? [errors.senha] : undefined} />
            </Field>

            <div className="flex items-center justify-between">
              <label className="flex items-center gap-2 text-sm text-neutral-600">
                <Checkbox
                  checked={watch("lembrarMe")}
                  onCheckedChange={(checked) => setValue("lembrarMe", checked === true)}
                />
                Lembrar-me
              </label>
              <button
                type="button"
                className="text-sm font-medium text-sus-blue hover:underline"
                onClick={() => toast.info("Funcionalidade ainda nao disponivel neste prototipo.")}
              >
                Esqueci minha senha
              </button>
            </div>

            <Button type="submit" className="w-full bg-sus-blue hover:bg-sus-blue-dark" disabled={loginMutation.isPending}>
              <LogIn className="size-4" />
              {loginMutation.isPending ? "Entrando..." : "Entrar"}
            </Button>
          </FieldGroup>
        </form>

        <div className="mt-8 rounded-lg border border-sus-blue-light bg-sus-blue-light/50 p-3">
          <p className="mb-2 text-xs font-semibold text-sus-blue-dark">Contas de demonstracao (qualquer senha)</p>
          <ul className="space-y-1">
            {CONTAS_DEMO.map((conta) => (
              <li key={conta.email}>
                <button
                  type="button"
                  className="text-xs text-sus-blue-dark/80 hover:underline"
                  onClick={() => {
                    setValue("email", conta.email);
                    setValue("senha", "demo123");
                  }}
                >
                  {conta.papel}: {conta.email}
                </button>
              </li>
            ))}
          </ul>
        </div>
      </div>
    </div>
  );
}
