import type { TipoSessao } from "./enums";

export type SessionFormFieldType = "text" | "textarea" | "number" | "date" | "select" | "boolean";

export interface SessionFormField {
  key: string;
  label: string;
  type: SessionFormFieldType;
  options?: string[];
  required?: boolean;
}

export interface SessionFormSchema {
  sessionType: TipoSessao;
  title: string;
  fields: SessionFormField[];
}
