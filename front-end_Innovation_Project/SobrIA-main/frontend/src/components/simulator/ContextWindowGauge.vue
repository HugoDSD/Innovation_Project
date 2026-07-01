<script setup lang="ts">
import { computed } from 'vue'
import type { EventCategory } from '@/types'
import { CATEGORIES, CATEGORY_ORDER } from '@/data/categories'
import { formatInt, formatTokens } from '@/lib/format'

const props = defineProps<{
  byCategory: Record<EventCategory, number>
  total: number
  max: number
  inputTokens: number
  outputTokens: number
}>()

const pct = computed(() => Math.min(100, (props.total / props.max) * 100))

/** Zone follows the 50 % / 75 % thresholds, like the Claude Code gauge. */
const zone = computed<'good' | 'warn' | 'bad'>(() => {
  if (pct.value >= 75) return 'bad'
  if (pct.value >= 50) return 'warn'
  return 'good'
})

const zoneColor = computed(
  () =>
    ({
      good: 'var(--color-verdict-good)',
      warn: 'var(--color-verdict-warn)',
      bad: 'var(--color-verdict-bad)',
    })[zone.value],
)

const zoneLabel = computed(
  () => ({ good: 'Sous 50 %', warn: '50–75 %', bad: 'Au-dessus de 75 %' })[zone.value],
)

const segments = computed(() =>
  CATEGORY_ORDER.filter((c) => props.byCategory[c] > 0).map((c) => ({
    cat: CATEGORIES[c],
    tokens: props.byCategory[c],
    widthPct: (props.byCategory[c] / props.max) * 100,
  })),
)
</script>

<template>
  <div class="rounded-2xl border border-forest-200 bg-white/70 p-5 shadow-sm">
    <div class="flex items-end justify-between mb-4">
      <div>
        <p class="text-xs uppercase tracking-wider text-ink-400 font-600">Fenêtre de contexte</p>
        <p class="text-sm text-ink-500">
          {{ formatInt(total) }} / {{ formatInt(max) }} tokens
        </p>
      </div>
      <div class="text-right">
        <p class="font-display text-4xl font-700 tabular-nums" :style="{ color: zoneColor }">
          {{ pct.toFixed(1) }}%
        </p>
        <p class="text-xs font-600" :style="{ color: zoneColor }">{{ zoneLabel }}</p>
      </div>
    </div>

    <!-- Stacked track -->
    <div class="relative h-9 rounded-lg bg-paper-100 ring-1 ring-forest-200/70 overflow-hidden">
      <div class="flex h-full w-full">
        <div
          v-for="seg in segments"
          :key="seg.cat.key"
          class="h-full transition-all duration-300 ease-out"
          :style="{ width: `${seg.widthPct}%`, backgroundColor: seg.cat.color }"
          :title="`${seg.cat.label} · ${formatInt(seg.tokens)} tokens`"
        />
      </div>

      <!-- Threshold markers at 50 % and 75 % -->
      <div
        class="absolute top-0 bottom-0 w-px bg-ink-900/30"
        style="left: 50%"
        aria-hidden="true"
      >
        <span class="absolute -top-0.5 left-1 text-[10px] text-ink-400">50%</span>
      </div>
      <div
        class="absolute top-0 bottom-0 w-px bg-ink-900/30"
        style="left: 75%"
        aria-hidden="true"
      >
        <span class="absolute -top-0.5 left-1 text-[10px] text-ink-400">75%</span>
      </div>
    </div>

    <!-- Role split -->
    <div class="mt-3 flex gap-4 text-xs text-ink-500">
      <span class="inline-flex items-center gap-1.5">
        <span class="h-2 w-2 rounded-full bg-forest-500" />
        Entrée <strong class="text-ink-700 tabular-nums">{{ formatTokens(inputTokens) }}</strong>
      </span>
      <span class="inline-flex items-center gap-1.5">
        <span class="h-2 w-2 rounded-full bg-forest-300" />
        Sortie <strong class="text-ink-700 tabular-nums">{{ formatTokens(outputTokens) }}</strong>
      </span>
      <span class="ml-auto text-ink-400">Facturation « A » : occupation finale</span>
    </div>
  </div>
</template>
