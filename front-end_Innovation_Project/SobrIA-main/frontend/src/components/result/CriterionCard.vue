<script setup lang="ts">
import type { Rating } from '@/types'

defineProps<{
  title: string
  kind: 'bénéfice' | 'coût'
  rating: Rating
  metrics: { label: string; value: string }[]
}>()

const dots = [1, 2, 3, 4, 5]
</script>

<template>
  <div class="rounded-2xl border border-forest-200 bg-white/70 p-5 shadow-sm">
    <div class="flex items-center justify-between mb-3">
      <div>
        <h3 class="font-display text-lg font-600 text-ink-900">{{ title }}</h3>
        <span
          class="text-[11px] uppercase tracking-wider font-600"
          :class="kind === 'bénéfice' ? 'text-forest-500' : 'text-ink-400'"
        >
          {{ kind }}
        </span>
      </div>
      <div class="text-right">
        <span class="font-display text-2xl font-700 text-forest-600 tabular-nums">{{ rating }}</span>
        <span class="text-sm text-ink-400">/5</span>
      </div>
    </div>

    <!-- Rating dots -->
    <div class="flex gap-1 mb-4" :aria-label="`Note ${rating} sur 5`">
      <span
        v-for="d in dots"
        :key="d"
        class="h-1.5 flex-1 rounded-full transition-colors"
        :class="d <= rating ? 'bg-forest-500' : 'bg-forest-100'"
      />
    </div>

    <dl class="space-y-1.5 text-sm">
      <div v-for="m in metrics" :key="m.label" class="flex items-baseline justify-between gap-3">
        <dt class="text-ink-500">{{ m.label }}</dt>
        <dd class="text-ink-800 font-500 tabular-nums text-right">{{ m.value }}</dd>
      </div>
    </dl>
  </div>
</template>
