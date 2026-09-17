import { HttpResponse, http } from "msw";
import { db } from "../db";

export const legalBasisHandlers = [http.get("*/api/legal-basis", () => HttpResponse.json(db.legalBasis))];
