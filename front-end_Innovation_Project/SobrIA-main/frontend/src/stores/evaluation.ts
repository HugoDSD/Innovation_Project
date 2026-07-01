/**
 * Current-evaluation store — a lightweight reactive singleton that carries state
 * across the three screens (form → simulator → result). Avoids pulling in Pinia
 * for a single flow.
 *
 * Pipeline (mirrors the 4 steps of `docs/MODELE-EVALUATION.md`):
 *   form input → generate timeline → derive tokens → derive variables →
 *   score 1–5 → verdict → persist to history.
 */
import { reactive } from 'vue'
import type { Evaluation, SessionTimeline, WorkflowInput } from '@/types'
import { getModel } from '@/data/catalog'
import { deriveDecisionVariables, generateSessionTimeline } from '@/data/stubs'
import { deriveTokenUsage } from '@/data/timeline'
import { scoreAll } from '@/scoring/ratings'
import { computeVerdict } from '@/scoring/verdict'
import { useHistory } from './history'

interface EvaluationState {
  input: WorkflowInput | null
  timeline: SessionTimeline | null
  result: Evaluation | null
}

const state = reactive<EvaluationState>({
  input: null,
  timeline: null,
  result: null,
})

function makeId(): string {
  return `eval-${Date.now().toString(36)}-${Math.random().toString(36).slice(2, 7)}`
}

/** Screen 1 → screen 2: capture the form and build the (stub) session timeline. */
export function startEvaluation(input: WorkflowInput): SessionTimeline {
  state.input = input
  state.result = null
  state.timeline = generateSessionTimeline(getModel(input.aiModelId))
  return state.timeline
}

/**
 * Screen 2 → screen 3: run the rest of the pipeline on the captured input and
 * the simulated timeline, then persist. Returns the finished evaluation.
 */
export function finalizeEvaluation(): Evaluation {
  if (!state.input || !state.timeline) {
    throw new Error('finalizeEvaluation called before startEvaluation')
  }
  const tokens = deriveTokenUsage(state.timeline)
  const variables = deriveDecisionVariables(state.input, tokens)
  const ratings = scoreAll(variables)
  const verdict = computeVerdict(ratings)

  const evaluation: Evaluation = {
    id: makeId(),
    createdAt: new Date().toISOString(),
    input: state.input,
    timeline: state.timeline,
    variables,
    ratings,
    verdict,
  }

  state.result = evaluation
  useHistory().add(evaluation)
  return evaluation
}

export function useEvaluation() {
  return {
    state,
    startEvaluation,
    finalizeEvaluation,
  }
}
