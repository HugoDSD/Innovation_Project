/**
 * Domain type contracts for SobrIA.
 *
 * These mirror the reference tables in `docs/MODELE-EVALUATION.md` and the
 * agent-session data contract in `docs/PLAN-IMPLEMENTATION.md`. In Phase 1 every
 * decision variable is a random stub; only the 1–5 rating rules and the verdict
 * cascade carry real logic. The shapes here are intentionally source-agnostic so
 * Phases 2–3 can swap a stub for a formula or an LLM call, variable by variable.
 */

/* ------------------------------------------------------------------ *
 * Screen 1 — workflow form inputs
 * ------------------------------------------------------------------ */

export type ExperienceLevel = 'junior' | 'confirmé' | 'senior' | 'expert'

export interface WorkflowInput {
  /** Free-text task description and what it replaces. */
  workflowDescription: string
  /** Number of runs over a period (e.g. per month). */
  runFrequency: number
  /** Number of employees involved in this workflow per run. */
  employeeCount: number
  /** Hours each employee spends on this workflow per single run (before AI). */
  hoursPerRun: number
  /** Experience level of the employees — used to derive an hourly rate. */
  experienceLevel: ExperienceLevel
  /** Chosen AI model id (see catalog in `data/catalog.ts`). */
  aiModelId: string
  /** Chosen cloud provider / region id. */
  complexity: string
  cloudProviderId: string

  useCase: string 
  dataSensitivity: DataSensitivity
  legalRisk: LegalRisk
  aiSavingsFraction: number
}

export interface RecommendationBlock {
  model: string
  complexity: string
  energyKwh: number
  waterLiters: number
  costUsd: number
}


/* ------------------------------------------------------------------ *
 * Model & provider metadata (stubbed now, models.dev in a later phase)
 * ------------------------------------------------------------------ */

export interface AiModel {
  id: string
  name: string
  vendor: string
  /** Context window size in tokens — the simulator gauge MAX. */
  contextWindow: number
  /** USD per input token. */
  priceIn: number
  /** USD per output token. */
  priceOut: number
}

export interface CloudProvider {
  id: string
  name: string
  region: string
  /** gCO₂ per kWh. */
  carbonIntensity: number
  /** Litres of water per kWh. */
  waterFactor: number
  /** Wh consumed per 1k tokens (rough energy proxy). */
  whPerKToken: number
}

/* ------------------------------------------------------------------ *
 * Screen 2 — agent-session token simulator
 * ------------------------------------------------------------------ */

/** Categories shown in the context-window legend. */
export type EventCategory =
  | 'system'
  | 'claudeMd'
  | 'memory'
  | 'skills'
  | 'mcp'
  | 'rules'
  | 'user'
  | 'files'
  | 'output'
  | 'claude'
  | 'hooks'

/** Whether tokens count as billable input or output. */
export type EventRole = 'input' | 'output'

export type EventKind =
  | 'prompt'
  | 'fileRead'
  | 'response'
  | 'toolOutput'
  | 'hook'
  | 'subagent'
  | 'compaction'
  | 'system'

/** One ordered event in the agent session timeline. */
export interface SessionEvent {
  order: number
  kind: EventKind
  category: EventCategory
  label: string
  /** Token cost of this event in the main window. */
  tokens: number
  role: EventRole
  /** Whether the event is shown in the conversation (visibilité). */
  visible: boolean
  /** Turn index this event belongs to (turns stack, they don't multiply). */
  turn: number
  /**
   * Tokens consumed inside a subagent's *separate* window — these do NOT count
   * toward the main total. Only set for `kind === 'subagent'`.
   */
  subTokens?: number
}

export interface SessionTimeline {
  events: SessionEvent[]
  /** Context window size of the chosen model. */
  maxTokens: number
}

/* ------------------------------------------------------------------ *
 * Decision variables (step 1 + step 2 of the model)
 * ------------------------------------------------------------------ */

export type DataSensitivity = 'public' | 'interne' | 'confidentiel' | 'réglementé'
export type LegalRisk = 'faible' | 'modéré' | 'élevé' | 'critique'

export interface TokenUsage {
  /** Input-role tokens occupying the window at session end (occupancy "A"). */
  inputTokens: number
  /** Output-role tokens occupying the window at session end. */
  outputTokens: number
  /** Number of turns, derived from the timeline. */
  turns: number
}

export interface EfficiencyVars {
  /** Fraction of hoursPerRun × employeeCount that AI saves (0–1). Inferred by LLM in Phase 3. */
  aiSavingsFraction: number
  /** Total person-hours saved per run = fraction × hoursPerRun × employeeCount. */
  hoursSavedPerRun: number
  /** €/h derived from experienceLevel — never asked directly. */
  hourlyRate: number
  /** € = hoursSavedPerRun × hourlyRate × runFrequency. */
  valueSaved: number
}

export interface EnvironmentalVars {
  energyKwh: number
  co2Kg: number
  waterL: number
}

export interface EconomicVars {
  costUsdPerRun: number
  costUsdTotal: number
}

export interface RiskVars {
  dataSensitivity: DataSensitivity
  legalRisk: LegalRisk
}

export interface DecisionVariables {
  tokens: TokenUsage
  efficiency: EfficiencyVars
  environmental: EnvironmentalVars
  economic: EconomicVars
  risk: RiskVars
}

/* ------------------------------------------------------------------ *
 * Step 3 — scoring, Step 4 — verdict
 * ------------------------------------------------------------------ */

/** 1–5, uniform polarity: 5 = most favorable to AI, 1 = least. */
export type Rating = 1 | 2 | 3 | 4 | 5

export type CriterionKey = 'efficiency' | 'environmental' | 'economic' | 'risk'

export type Ratings = Record<CriterionKey, Rating>

export type VerdictLevel = 'Recommandé' | 'À optimiser' | 'Déconseillé'

export interface Verdict {
  level: VerdictLevel
  /** Which cascade gate fired (for explainability). */
  gate: string
  /** Human-readable reason, e.g. "Risque juridique élevé (Risque 1/5)". */
  reason: string
}

/* ------------------------------------------------------------------ *
 * The full evaluation (persisted to history)
 * ------------------------------------------------------------------ */

export interface Evaluation {
  id: string
  createdAt: string
  input: WorkflowInput
  timeline: SessionTimeline // On garde ça pour l'affichage visuel
  
  // --- DONNÉES RENVOYÉES PAR L'API C# ---
  evaluationId: number
  verdictLevel: string
  verdictReason: string
  gateTriggered: string
  
  efficiencyRating: number
  environmentalRating: number
  economicRating: number
  riskRating: number
  
  totalEnergyKwh: number
  totalCarbonKg: number
  totalWaterLiters: number
  totalCostUsd: number
  valueSavedEur: number
  
  recommendedEnv: RecommendationBlock
  recommendedEco: RecommendationBlock
  recommendedQuality: RecommendationBlock
}