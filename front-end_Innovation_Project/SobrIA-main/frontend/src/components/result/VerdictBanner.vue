<script setup lang="ts">
import { computed } from 'vue'
import type { Verdict } from '@/types'
import { VERDICT_STYLES } from '@/lib/labels'

const props = defineProps<{ verdict: Verdict }>()

const style = computed(() => VERDICT_STYLES[props.verdict.level])
</script>

<template>
  <div
    class="rounded-2xl border p-6 sm:p-7"
    :style="{ backgroundColor: style.bg, borderColor: style.border }"
  >
    <div class="flex items-start gap-4">
      <span
        class="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl text-2xl font-700"
        :style="{ backgroundColor: style.color, color: 'white' }"
      >
        {{ style.icon }}
      </span>
      <div class="min-w-0">
        <p class="text-xs uppercase tracking-wider font-600 mb-0.5" :style="{ color: style.color }">
          Verdict
        </p>
        <h2 class="font-display text-2xl sm:text-3xl font-700" :style="{ color: style.color }">
          {{ verdict.level }}
        </h2>
        <p class="mt-2 text-ink-700 leading-relaxed">{{ verdict.reason }}</p>
        <p class="mt-1 text-xs text-ink-400">Garde-fou déclenché : <code>{{ verdict.gate }}</code></p>
      </div>
    </div>
  </div>
</template>
