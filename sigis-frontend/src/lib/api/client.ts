import axios from "axios";
import { useAuthStore } from "@/stores/authStore";
import { toApiError } from "./errors";

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_URL ?? "http://localhost:5000",
  timeout: 30_000,
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token;
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  config.headers["X-Request-Id"] = crypto.randomUUID();
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const config = error.config;

    // Retry único em erro de rede (sem resposta do servidor) — nunca em 4xx/5xx,
    // que são falhas de negócio/servidor e não devem ser repetidas às cegas.
    if (!error.response && config && !config.__retried) {
      config.__retried = true;
      return apiClient(config);
    }

    if (error.response?.status === 401) {
      useAuthStore.getState().logout();
      if (window.location.pathname !== "/login") {
        window.location.assign("/login");
      }
    }

    const apiError = toApiError(error.response?.status, error.response?.data, error.message ?? "Erro de comunicação com o servidor.");
    return Promise.reject(apiError);
  },
);
