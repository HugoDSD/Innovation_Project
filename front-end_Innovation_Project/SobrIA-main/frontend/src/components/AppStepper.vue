<script setup lang="ts">
import { computed } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

const steps = [
  { name: 'form', label: 'Workflow' },
  { name: 'simulation', label: 'Simulation' },
  { name: 'result', label: 'Résultat' },
]

// Map the active route to a step index; hide the stepper outside the flow.
const activeIndex = computed(() => {
  const idx = steps.findIndex((s) => s.name === route.name)
  return idx
})

const visible = computed(() => activeIndex.value !== -1)
</script>

<template>
  <div v-if="visible" class="border-b border-forest-200/40 bg-paper-100/50">
    <ol class="mx-auto max-w-5xl px-5 py-3 flex items-center gap-2 text-sm">
      <template v-for="(step, i) in steps" :key="step.name">
        <li class="flex items-center gap-2">
          <span
            class="flex h-6 w-6 items-center justify-center rounded-full text-xs font-600 transition-colors"
            :class="
              i < activeIndex
                ? 'bg-forest-600 text-paper'
                : i === activeIndex
                  ? 'bg-forest-100 text-forest-700 ring-2 ring-forest-400'
                  : 'bg-paper-100 text-ink-400 ring-1 ring-forest-200'
            "
          >
            <span v-if="i < activeIndex">✓</span>
            <span v-else>{{ i + 1 }}</span>
          </span>
          <span
            :class="i === activeIndex ? 'text-forest-700 font-600' : 'text-ink-400'"
            class="hidden sm:inline"
          >
            {{ step.label }}
          </span>
        </li>
        <li v-if="i < steps.length - 1" class="flex-1 h-px bg-forest-200/70 min-w-4" aria-hidden="true" />
      </template>
    </ol>
  </div>
</template>
