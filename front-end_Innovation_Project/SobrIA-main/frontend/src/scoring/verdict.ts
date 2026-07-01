/**
 * Step 4 — the verdict cascade (REAL logic).
 *
 * Not a weighted average: an ordered set of guardrails (`gates`) is checked in
 * order and the FIRST match decides. This stops a good score on one axis from
 * masking a deal-breaker on another. See `docs/MODELE-EVALUATION.md` › "La
 * logique de décision".
 *
 *   Gate 1  — Risque ≤ 2                            → Déconseillé (veto absolu)
 *   Gate 2  — Efficacité ≤ 2                        → Déconseillé (bénéfice trop faible)
 *   Gate 3a — Environnemental ≤ 2 ET Économique ≤ 2 → Déconseillé (double dépassement)
 *   Gate 3b — Environnemental ≤ 2 OU Économique ≤ 2 → À optimiser
 *   Sinon                                           → Recommandé
 */
import type { Ratings, Verdict } from '@/types'

/** Ratings at or below this fire a guardrail. Calibratable. */
export const GATE_THRESHOLD = 2

export function computeVerdict(r: Ratings): Verdict {
  if (r.risk <= GATE_THRESHOLD) {
    return {
      level: 'Déconseillé',
      gate: 'risk-veto',
      reason: `Risque trop élevé (Risque ${r.risk}/5) — un risque élevé n'est pas compensable par du temps gagné.`,
    }
  }

  if (r.efficiency <= GATE_THRESHOLD) {
    return {
      level: 'Déconseillé',
      gate: 'efficiency-floor',
      reason: `Gain de temps négligeable (Efficacité ${r.efficiency}/5) — sans bénéfice, rien à arbitrer.`,
    }
  }

  if (r.environmental <= GATE_THRESHOLD && r.economic <= GATE_THRESHOLD) {
    return {
      level: 'Déconseillé',
      gate: 'double-cost',
      reason: `Empreinte environnementale et coût tous deux trop élevés (Environnemental ${r.environmental}/5, Économique ${r.economic}/5) — optimiser ne suffit plus, reconsidérer l'usage.`,
    }
  }

  if (r.environmental <= GATE_THRESHOLD || r.economic <= GATE_THRESHOLD) {
    const culprit =
      r.environmental <= GATE_THRESHOLD
        ? `empreinte environnementale élevée (Environnemental ${r.environmental}/5)`
        : `coût élevé (Économique ${r.economic}/5)`
    return {
      level: 'À optimiser',
      gate: 'cost-or-footprint',
      reason: `Usage utile et sûr mais ${culprit} — à employer avec sobriété.`,
    }
  }

  return {
    level: 'Recommandé',
    gate: 'pass',
    reason: 'La valeur justifie l\'impact sur les quatre critères — bon choix pour ce workflow.',
  }
}
