import { HttpResponse, http } from "msw";
import { db } from "../db";

export const catalogosHandlers = [
  http.get("*/api/unidades", () => HttpResponse.json(db.unidades)),
  http.get("*/api/profissionais", () => HttpResponse.json(db.profissionais)),
];
