/**
 * Phase 1 stub data layer.
 *
 * Everything here is RANDOM and stands behind a small interface so Phases 2–3
 * can replace it variable by variable:
 *   - `generateSessionTimeline` → Phase 3 swaps in an LLM-inferred timeline.
 *   - `DecisionVariableSource`   → Phase 2 swaps the physical groups for real
 *      formulas, Phase 3 swaps the fuzzy groups for LLM calls.
 *
 * Token usage is NOT stubbed: it is derived for real from the (stub) timeline by
 * `deriveTokenUsage` — that is the simulator's genuine output (billing "A").
 */
import type {
  AiModel,
  CloudProvider,
  DataSensitivity,
  DecisionVariables,
  EconomicVars,
  EfficiencyVars,
  EnvironmentalVars,
  LegalRisk,
  RiskVars,
  SessionEvent,
  SessionTimeline,
  TokenUsage,
  WorkflowInput,
} from '@/types'
import { getModel, getProvider, HOURLY_RATE_BY_LEVEL } from './catalog'

/* ------------------------------ randomness ------------------------------ */

const rand = (min: number, max: number) => min + Math.random() * (max - min)
const randInt = (min: number, max: number) => Math.floor(rand(min, max + 1))
const pick = <T>(xs: readonly T[]): T => xs[randInt(0, xs.length - 1)]!

/* --------------------------- session timeline --------------------------- */

let eventOrder = 0
function makeEvent(
  e: Omit<SessionEvent, 'order'>,
): SessionEvent {
  return { order: eventOrder++, ...e }
}

/**
 * Build a plausible multi-turn agent session. Startup content loads once, then
 * each user turn stacks a prompt, optional file reads / tool outputs / hooks /
 * subagent, and a model response. A /compact may fire mid-session.
 */
export function generateSessionTimeline(model: AiModel): SessionTimeline {
  eventOrder = 0
  const events: SessionEvent[] = []
  const max = model.contextWindow

  // Turn 0 — startup (persistent across compaction).
  events.push(
    makeEvent({ kind: 'system', category: 'system', label: 'Prompt système', tokens: randInt(2200, 3200), role: 'input', visible: false, turn: 0 }),
    makeEvent({ kind: 'system', category: 'claudeMd', label: 'CLAUDE.md', tokens: randInt(600, 1800), role: 'input', visible: false, turn: 0 }),
    makeEvent({ kind: 'system', category: 'memory', label: 'Fichiers mémoire', tokens: randInt(300, 1200), role: 'input', visible: false, turn: 0 }),
    makeEvent({ kind: 'system', category: 'skills', label: 'Compétences', tokens: randInt(400, 1600), role: 'input', visible: false, turn: 0 }),
    makeEvent({ kind: 'system', category: 'mcp', label: 'Outils MCP', tokens: randInt(500, 2400), role: 'input', visible: false, turn: 0 }),
    makeEvent({ kind: 'system', category: 'rules', label: 'Règles du projet', tokens: randInt(200, 900), role: 'input', visible: false, turn: 0 }),
  )

  const turns = randInt(3, 7)
  const compactionTurn = Math.random() < 0.5 ? randInt(2, turns - 1) : -1

  for (let turn = 1; turn <= turns; turn++) {
    if (turn === compactionTurn) {
      events.push(
        makeEvent({ kind: 'compaction', category: 'system', label: '/compact — résumé de la conversation', tokens: 0, role: 'input', visible: true, turn }),
      )
    }

    events.push(
      makeEvent({ kind: 'prompt', category: 'user', label: `Message utilisateur ${turn}`, tokens: randInt(80, 700), role: 'input', visible: true, turn }),
    )

    for (let f = 0; f < randInt(0, 3); f++) {
      events.push(
        makeEvent({ kind: 'fileRead', category: 'files', label: `Lecture fichier`, tokens: randInt(400, 4500), role: 'input', visible: true, turn }),
      )
    }

    if (Math.random() < 0.5) {
      events.push(
        makeEvent({ kind: 'toolOutput', category: 'output', label: 'Sortie outil', tokens: randInt(200, 2600), role: 'input', visible: true, turn }),
      )
    }

    if (Math.random() < 0.3) {
      events.push(
        makeEvent({ kind: 'hook', category: 'hooks', label: 'Hook', tokens: randInt(50, 400), role: 'input', visible: false, turn }),
      )
    }

    if (Math.random() < 0.35) {
      events.push(
        makeEvent({ kind: 'subagent', category: 'claude', label: 'Sous-agent', tokens: 0, subTokens: randInt(8000, 40000), role: 'input', visible: true, turn }),
      )
    }

    events.push(
      makeEvent({ kind: 'response', category: 'claude', label: `Réponse de Claude ${turn}`, tokens: randInt(300, 2400), role: 'output', visible: true, turn }),
    )
  }

  return { events, maxTokens: max }
}

/* ----------------------- decision-variable source ----------------------- */

export interface VariableContext {
  input: WorkflowInput
  tokens: TokenUsage
  model: AiModel
  provider: CloudProvider
}

/**
 * Pluggable source for the four criteria's decision variables. Phase 1 ships
 * `stubSource`; later phases provide formula/LLM-backed implementations with the
 * same shape and swap them in here.
 */
export interface DecisionVariableSource {
  efficiency(ctx: VariableContext): EfficiencyVars
  environmental(ctx: VariableContext): EnvironmentalVars
  economic(ctx: VariableContext): EconomicVars
  risk(ctx: VariableContext): RiskVars
}

const SENSITIVITY: DataSensitivity[] = ['public', 'interne', 'confidentiel', 'réglementé']
const LEGAL: LegalRisk[] = ['faible', 'modéré', 'élevé', 'critique']

export const stubSource: DecisionVariableSource = {
  efficiency({ input }): EfficiencyVars {
    const hourlyRate = HOURLY_RATE_BY_LEVEL[input.experienceLevel]
    const totalPersonHours = input.employeeCount * input.hoursPerRun
    // Stub: AI saves between 20% and 90% of person-hours. Phase 3 will infer this via LLM.
    const aiSavingsFraction = Number(rand(0.2, 0.9).toFixed(2))
    const hoursSavedPerRun = Number((aiSavingsFraction * totalPersonHours).toFixed(2))
    const valueSaved = Math.round(hoursSavedPerRun * hourlyRate * input.runFrequency)
    return { aiSavingsFraction, hoursSavedPerRun, hourlyRate, valueSaved }
  },

  environmental({ tokens, provider, input }): EnvironmentalVars {
    // Stub, but anchored to the real token count so the figure tracks the run.
    const kTokens = (tokens.inputTokens + tokens.outputTokens) / 1000
    const perRunKwh = (kTokens * provider.whPerKToken) / 1000
    const energyKwh = Number((perRunKwh * input.runFrequency * rand(0.8, 1.3)).toFixed(4))
    const co2Kg = Number((energyKwh * (provider.carbonIntensity / 1000)).toFixed(4))
    const waterL = Number((energyKwh * provider.waterFactor).toFixed(3))
    return { energyKwh, co2Kg, waterL }
  },

  economic({ tokens, model, input }): EconomicVars {
    const costUsdPerRun = Number(
      (tokens.inputTokens * model.priceIn + tokens.outputTokens * model.priceOut).toFixed(4),
    )
    const costUsdTotal = Number((costUsdPerRun * input.runFrequency).toFixed(2))
    return { costUsdPerRun, costUsdTotal }
  },

  risk(): RiskVars {
    const dataSensitivity = pick(SENSITIVITY)
    const legalRisk = pick(LEGAL)
    return { dataSensitivity, legalRisk }
  },
}

export function deriveDecisionVariables(
  input: WorkflowInput,
  tokens: TokenUsage,
  source: DecisionVariableSource = stubSource,
): DecisionVariables {
  const ctx: VariableContext = {
    input,
    tokens,
    model: getModel(input.aiModelId),
    provider: getProvider(input.cloudProviderId),
  }
  return {
    tokens,
    efficiency: source.efficiency(ctx),
    environmental: source.environmental(ctx),
    economic: source.economic(ctx),
    risk: source.risk(ctx),
  }
}
