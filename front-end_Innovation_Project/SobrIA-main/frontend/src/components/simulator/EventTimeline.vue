<script setup lang="ts">
import type { SessionEvent } from '@/types'
import { CATEGORIES } from '@/data/categories'
import { formatInt } from '@/lib/format'

defineProps<{
  events: SessionEvent[]
}>()

const KIND_ICON: Record<string, string> = {
  prompt: '▸',
  fileRead: '📄',
  response: '✦',
  toolOutput: '⚙',
  hook: '⎇',
  subagent: '⊕',
  compaction: '⟳',
  system: '◆',
}
</script>

<template>
  <div class="rounded-2xl border border-forest-200 bg-white/70 shadow-sm overflow-hidden">
    <div class="px-5 py-3 border-b border-forest-200/60 flex items-center justify-between">
      <p class="text-xs uppercase tracking-wider text-ink-400 font-600">Timeline d'événements</p>
      <p class="text-xs text-ink-400 tabular-nums">{{ events.length }} événements</p>
    </div>

    <ol class="max-h-[28rem] overflow-y-auto divide-y divide-forest-100">
      <li
        v-for="event in events"
        :key="event.order"
        class="px-5 py-2.5 flex items-center gap-3 animate-rise"
        :class="event.kind === 'compaction' ? 'bg-forest-50/60' : ''"
      >
        <span
          class="flex h-7 w-7 items-center justify-center rounded-lg text-xs shrink-0"
          :style="{
            backgroundColor: `${CATEGORIES[event.category].color}1a`,
            color: CATEGORIES[event.category].color,
          }"
        >
          {{ KIND_ICON[event.kind] ?? '•' }}
        </span>

        <div class="min-w-0 flex-1">
          <p class="text-sm text-ink-800 truncate">{{ event.label }}</p>
          <p class="text-[11px] text-ink-400">
            Tour {{ event.turn }} · {{ CATEGORIES[event.category].label }}
          </p>
        </div>

        <div class="text-right shrink-0">
          <template v-if="event.kind === 'subagent'">
            <span class="text-xs font-600 text-ink-500 tabular-nums">
              {{ formatInt(event.subTokens ?? 0) }}
            </span>
            <p class="text-[10px] text-ink-400">fenêtre séparée</p>
          </template>
          <template v-else-if="event.kind === 'compaction'">
            <span class="text-xs font-600 text-forest-600">résumé ~12%</span>
          </template>
          <template v-else>
            <span class="text-sm font-600 text-ink-700 tabular-nums">+{{ formatInt(event.tokens) }}</span>
            <p
              class="text-[10px]"
              :class="event.role === 'output' ? 'text-forest-400' : 'text-ink-400'"
            >
              {{ event.role === 'output' ? 'sortie' : 'entrée' }}
            </p>
          </template>
        </div>
      </li>
    </ol>
  </div>
</template>
