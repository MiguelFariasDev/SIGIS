import { useMutation } from "@tanstack/react-query";
import { useAuthStore } from "@/stores/authStore";
import { login } from "./api";

export function useLogin() {
  const setAuth = useAuthStore((s) => s.login);

  return useMutation({
    mutationFn: login,
    onSuccess: (data) => {
      setAuth(data.token, data.profissional);
    },
  });
}
