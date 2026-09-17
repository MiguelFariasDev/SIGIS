/**
 * Hierarquia de erros da API real. O backend responde erros como
 * `{ code: string, message: string }` (ex.: `{"code":"AUTH_001","message":"Credenciais inválidas."}`).
 * O interceptor de resposta em `client.ts` converte o erro do axios numa
 * destas classes antes de rejeitar, para que quem chama possa usar
 * `instanceof` em vez de checar `error.response.status` na mão.
 */
export class ApiError extends Error {
  code?: string;
  details?: unknown;

  constructor(message: string, code?: string, details?: unknown) {
    super(message);
    this.name = "ApiError";
    this.code = code;
    this.details = details;
  }
}

export class ValidationError extends ApiError {
  constructor(message: string, code?: string, details?: unknown) {
    super(message, code, details);
    this.name = "ValidationError";
  }
}

export class UnauthorizedError extends ApiError {
  constructor(message: string, code?: string, details?: unknown) {
    super(message, code, details);
    this.name = "UnauthorizedError";
  }
}

export class ForbiddenError extends ApiError {
  constructor(message: string, code?: string, details?: unknown) {
    super(message, code, details);
    this.name = "ForbiddenError";
  }
}

export class NotFoundError extends ApiError {
  constructor(message: string, code?: string, details?: unknown) {
    super(message, code, details);
    this.name = "NotFoundError";
  }
}

export class ConflictError extends ApiError {
  constructor(message: string, code?: string, details?: unknown) {
    super(message, code, details);
    this.name = "ConflictError";
  }
}

export class ServerError extends ApiError {
  constructor(message: string, code?: string, details?: unknown) {
    super(message, code, details);
    this.name = "ServerError";
  }
}

interface BackendErrorBody {
  code?: string;
  message?: string;
}

/** Constrói a subclasse de `ApiError` correta a partir do status HTTP e do corpo do erro do backend. */
export function toApiError(status: number | undefined, body: BackendErrorBody | undefined, fallbackMessage: string): ApiError {
  const message = body?.message ?? fallbackMessage;
  const code = body?.code;

  switch (status) {
    case 400:
      return new ValidationError(message, code, body);
    case 401:
      return new UnauthorizedError(message, code, body);
    case 403:
      return new ForbiddenError(message, code, body);
    case 404:
      return new NotFoundError(message, code, body);
    case 409:
      return new ConflictError(message, code, body);
    case 500:
      return new ServerError(message, code, body);
    default:
      return new ApiError(message, code, body);
  }
}
