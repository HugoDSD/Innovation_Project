/**
 * Context-window occupancy replay.
 *
 * The simulator accumulates events into a single window. Turns STACK, they do
 * not multiply. `/compact` collapses the conversation so far into a ~12 % summary
 * while the startup content (system, CLAUDE.md, memory, skills, MCP, rules)
 * reloads and persists. Subagent reads live in a separate window and never count
 * toward the main total.
 *
 * Billing is choice "A": `inputTokens` / `outputTokens` are the FINAL occupancy
 * of the window by role — exactly what the gauge shows at the end.
 */
import type { EventCategory, SessionEvent, SessionTimeline, TokenUsage } from '@/types'
import { CATEGORY_ORDER } from './categories'

/** Categories loaded at startup that survive a /compact. */
const PERSISTENT: ReadonlySet<EventCategory> = new Set<EventCategory>([
  'system',
  'claudeMd',
  'memory',
  'skills',
  'mcp',
  'rules',
])

export interface OccupancySnapshot {
  /** The event that produced this snapshot. */
  event: SessionEvent
  total: number
  inputTokens: number
  outputTokens: number
  byCategory: Record<EventCategory, number>
  /** Tokens currently held in the subagent's separate window. */
  subagentTokens: number
}

function emptyBuckets(): Record<EventCategory, number> {
  return Object.fromEntries(CATEGORY_ORDER.map((c) => [c, 0])) as Record<EventCategory, number>
}

/**
 * Replay the timeline, yielding one occupancy snapshot per event. The last
 * snapshot is the final window state used for billing.
 */
export function occupancySeries(timeline: SessionTimeline): OccupancySnapshot[] {
  const buckets = emptyBuckets()
  let input = 0
  let output = 0
  let subagentTokens = 0
  const snapshots: OccupancySnapshot[] = []

  for (const event of timeline.events) {
    if (event.kind === 'subagent') {
      // Separate window — accumulates independently of the main total.
      subagentTokens += event.subTokens ?? 0
    } else if (event.kind === 'compaction') {
      // Collapse all NON-persistent (conversation) tokens into a summary.
      let summarized = 0
      for (const cat of CATEGORY_ORDER) {
        if (!PERSISTENT.has(cat)) {
          summarized += buckets[cat]
          buckets[cat] = 0
        }
      }
      const summary = Math.round(summarized * 0.12)
      buckets.system += summary
      // Recompute role splits from buckets: persistent + summary are input,
      // and the conversation output has been folded away.
      input = CATEGORY_ORDER.reduce((sum, c) => sum + buckets[c], 0)
      output = 0
      subagentTokens = 0 // subagent window is released after compaction
    } else {
      buckets[event.category] += event.tokens
      if (event.role === 'input') input += event.tokens
      else output += event.tokens
    }

    snapshots.push({
      event,
      total: input + output,
      inputTokens: input,
      outputTokens: output,
      byCategory: { ...buckets },
      subagentTokens,
    })
  }

  return snapshots
}

/** Final occupancy of the window, by role, plus the turn count. */
export function deriveTokenUsage(timeline: SessionTimeline): TokenUsage {
  const series = occupancySeries(timeline)
  const last = series.at(-1)
  const turns = timeline.events.reduce((max, e) => Math.max(max, e.turn), 0)
  return {
    inputTokens: last?.inputTokens ?? 0,
    outputTokens: last?.outputTokens ?? 0,
    turns,
  }
}
