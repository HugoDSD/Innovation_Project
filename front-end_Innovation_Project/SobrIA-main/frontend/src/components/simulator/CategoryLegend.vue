<script setup lang="ts">
import { computed } from 'vue'
import type { EventCategory } from '@/types'
import { CATEGORIES, CATEGORY_ORDER } from '@/data/categories'
import { formatInt } from '@/lib/format'

const props = defineProps<{
  byCategory: Record<EventCategory, number>
}>()

const rows = computed(() =>
  CATEGORY_ORDER.map((c) => ({
    cat: CATEGORIES[c],
    tokens: props.byCategory[c] ?? 0,
  })),
)
</script>

<template>
  <div class="rounded-2xl border border-forest-200 bg-white/70 p-5 shadow-sm">
    <p class="text-xs uppercase tracking-wider text-ink-400 font-600 mb-3">Catégories</p>
    <ul class="space-y-1.5">
      <li
        v-for="row in rows"
        :key="row.cat.key"
        class="flex items-center gap-2 text-sm"
        :class="row.tokens === 0 ? 'opacity-40' : ''"
      >
        <span class="h-3 w-3 rounded-sm shrink-0" :style="{ backgroundColor: row.cat.color }" />
        <span class="text-ink-700">{{ row.cat.label }}</span>
        <span class="ml-auto tabular-nums text-ink-500">{{ formatInt(row.tokens) }}</span>
      </li>
    </ul>
  </div>
</template>
