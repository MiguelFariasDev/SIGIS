export type Secretariat = "Health" | "Education" | "SocialAssistance";

export interface LegalBasis {
  id: string;
  code: string;
  description: string;
  secretariat: Secretariat;
  purpose: string;
  article: string;
}
