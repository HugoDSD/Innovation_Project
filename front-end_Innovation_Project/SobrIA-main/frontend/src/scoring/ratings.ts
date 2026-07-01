/**
 * Step 3 — map decision variables to a 1–5 rating per criterion.
 *
 * REAL logic (runs on stub values in Phase 1). Uniform polarity: 5 = most
 * favorable to AI, 1 = least. Thresholds are deliberately left as named,
 * single-source constants so they can be calibrated later without touching the
 * mapping logic (see `docs/MODELE-EVALUATION.md` › "Seuils" and the open
 * questions in the handoff).
 */
import type { DataSensitivity, DecisionVariables, LegalRisk, Rating, Ratings } from '@/types'

/**
 * Four ascending breakpoints `[t1, t2, t3, t4]`.
 * - "higher-better" (benefits): value ≥ t4 → 5, … , value < t1 → 1.
 * - "lower-better" (costs): value ≤ t1 → 5, … , value > t4 → 1.
 */
export type Thresholds = readonly [number, number, number, number]

/** Higher raw value → higher rating (used for benefits). */
export function rateHigherBetter(value: number, t: Thresholds): Rating {
  if (value >= t[3]) return 5
  if (value >= t[2]) return 4
  if (value >= t[1]) return 3
  if (value >= t[0]) return 2
  return 1
}

/** Higher raw value → lower rating (used for costs). */
export function rateLowerBetter(value: number, t: Thresholds): Rating {
  if (value <= t[0]) return 5
  if (value <= t[1]) return 4
  if (value <= t[2]) return 3
  if (value <= t[3]) return 2
  return 1
}

/**
 * Calibratable thresholds. PLACEHOLDER values for Phase 1 — the absolute
 * reference points (what € / kWh / $ = note 1 vs 5) are an open question in the
 * handoff. Keep them here as the single place to tune.
 */
export const THRESHOLDS = {
  /** Efficiency: total € value saved over the period (higher = better). */
  efficiencyValueSaved: [200, 800, 2500, 8000] as Thresholds,
  /** Environmental: total kg CO₂ over the period (lower = better). */
  environmentalCo2Kg: [0.5, 2, 8, 25] as Thresholds,
  /** Environmental: total litres of water over the period (lower = better). */
  environmentalWaterL: [15, 60, 250, 800] as Thresholds,
  /**
   * Economic: USD spent per EUR of value created (lower = better).
   * At 0.50 you spend 50¢ per €1 saved — rating 1.
   */
  economicCostRatio: [0.01, 0.05, 0.20, 0.50] as Thresholds,
} as const

/** Weight of CO₂ vs water in the environmental rating. Calibratable. */
export const ENVIRONMENTAL_WEIGHTS = { co2: 0.5, water: 0.5 } as const

const SENSITIVITY_ORDER: DataSensitivity[] = ['public', 'interne', 'confidentiel', 'réglementé']
const LEGAL_ORDER: LegalRisk[] = ['faible', 'modéré', 'élevé', 'critique']

/**
 * Risk dominance table: take the worse of the two ordinals, map directly.
 * Skips rating 3 to make the middle ground push toward "À optimiser".
 *
 *   worst = 0 (public / faible)        → 5
 *   worst = 1 (interne / modéré)       → 4
 *   worst = 2 (confidentiel / élevé)   → 2   (no rating 3)
 *   worst = 3 (réglementé / critique)  → 1   (veto)
 */
const RISK_DOMINANCE_TABLE: Rating[] = [5, 4, 2, 1]

export function rateEfficiency(v: DecisionVariables): Rating {
  return rateHigherBetter(v.efficiency.valueSaved, THRESHOLDS.efficiencyValueSaved)
}

export function rateEnvironmental(v: DecisionVariables): Rating {
  const co2Rating = rateLowerBetter(v.environmental.co2Kg, THRESHOLDS.environmentalCo2Kg)
  const waterRating = rateLowerBetter(v.environmental.waterL, THRESHOLDS.environmentalWaterL)
  const combined = ENVIRONMENTAL_WEIGHTS.co2 * co2Rating + ENVIRONMENTAL_WEIGHTS.water * waterRating
  return Math.round(combined) as Rating
}

export function costToValueRatio(v: DecisionVariables): number {
  if (v.efficiency.valueSaved <= 0) return Infinity
  return v.economic.costUsdTotal / v.efficiency.valueSaved
}

export function rateEconomic(v: DecisionVariables): Rating {
  return rateLowerBetter(costToValueRatio(v), THRESHOLDS.economicCostRatio)
}

export function rateRisk(v: DecisionVariables): Rating {
  const sIdx = SENSITIVITY_ORDER.indexOf(v.risk.dataSensitivity)
  const lIdx = LEGAL_ORDER.indexOf(v.risk.legalRisk)
  const worst = Math.max(sIdx, lIdx)
  return RISK_DOMINANCE_TABLE[worst] ?? 1
}

export function scoreAll(v: DecisionVariables): Ratings {
  return {
    efficiency: rateEfficiency(v),
    environmental: rateEnvironmental(v),
    economic: rateEconomic(v),
    risk: rateRisk(v),
  }
}
