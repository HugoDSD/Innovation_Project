<script setup lang="ts">
import { computed } from 'vue'
import { formatInt } from '@/lib/format'

const props = defineProps<{
  subagentTokens: number
  max: number
}>()

const pct = computed(() => Math.min(100, (props.subagentTokens / props.max) * 100))
const active = computed(() => props.subagentTokens > 0)
</script>

<template>
  <div
    class="rounded-2xl border border-dashed p-4 transition-colors"
    :class="active ? 'border-indigo-300 bg-indigo-50/40' : 'border-forest-200 bg-paper-100/40'"
  >
    <div class="flex items-center justify-between mb-2">
      <p class="text-xs uppercase tracking-wider font-600" :class="active ? 'text-indigo-500' : 'text-ink-400'">
        Fenêtre sous-agent
      </p>
      <span class="text-xs tabular-nums" :class="active ? 'text-indigo-600' : 'text-ink-400'">
        {{ formatInt(subagentTokens) }} tokens
      </span>
    </div>
    <div class="h-2 rounded-full bg-paper-100 ring-1 ring-forest-200/60 overflow-hidden">
      <div
        class="h-full rounded-full bg-indigo-400 transition-all duration-300"
        :style="{ width: `${pct}%` }"
      />
    </div>
    <p class="mt-2 text-[11px] text-ink-400">
      Les lectures des sous-agents vivent dans une fenêtre séparée — elles ne comptent pas dans le
      total principal.
    </p>
  </div>
</template>
