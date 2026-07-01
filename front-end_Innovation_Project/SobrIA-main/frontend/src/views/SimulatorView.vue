<script setup lang="ts">
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { finalizeEvaluation, useEvaluation } from '@/stores/evaluation'
import { occupancySeries } from '@/data/timeline'
import { getModel } from '@/data/catalog'
import { formatInt } from '@/lib/format'
import ContextWindowGauge from '@/components/simulator/ContextWindowGauge.vue'
import CategoryLegend from '@/components/simulator/CategoryLegend.vue'
import EventTimeline from '@/components/simulator/EventTimeline.vue'
import SubagentWindow from '@/components/simulator/SubagentWindow.vue'

const router = useRouter()
const { state } = useEvaluation()

const timeline = state.timeline!
const series = occupancySeries(timeline)
const model = computed(() => getModel(state.input!.aiModelId))

const step = ref(0) // index of the last played event
const playing = ref(true)
const speed = ref(1)
let timer: ReturnType<typeof setTimeout> | null = null

const snapshot = computed(() => series[step.value]!)
const visibleEvents = computed(() => timeline.events.slice(0, step.value + 1))
const atEnd = computed(() => step.value >= series.length - 1)
const progressPct = computed(() => ((step.value + 1) / series.length) * 100)

function clearTimer() {
  if (timer) {
    clearTimeout(timer)
    timer = null
  }
}

function tick() {
  clearTimer()
  if (!playing.value || atEnd.value) return
  timer = setTimeout(() => {
    step.value = Math.min(step.value + 1, series.length - 1)
    if (atEnd.value) playing.value = false
    else tick()
  }, 450 / speed.value)
}

function togglePlay() {
  if (atEnd.value) return
  playing.value = !playing.value
  if (playing.value) tick()
  else clearTimer()
}

function setSpeed(s: number) {
  speed.value = s
  if (playing.value) tick()
}

function skipToEnd() {
  clearTimer()
  playing.value = false
  step.value = series.length - 1
}

function restart() {
  clearTimer()
  step.value = 0
  playing.value = true
  tick()
}

function goToResult() {
  finalizeEvaluation()
  router.push({ name: 'result' })
}

onMounted(() => tick())
onUnmounted(clearTimer)
</script>

<template>
  <div class="mx-auto max-w-5xl px-5 py-10 animate-rise">
    <div class="mb-6 flex flex-wrap items-end justify-between gap-4">
      <div>
        <p class="text-sm font-600 uppercase tracking-wider text-forest-500 mb-2">Étape 2 · Simulation</p>
        <h1 class="text-3xl font-700 text-ink-900">Session d'agent simulée</h1>
        <p class="mt-1 text-ink-500">
          {{ model.name }} · fenêtre {{ formatInt(model.contextWindow) }} tokens — les événements
          s'empilent dans une fenêtre unique.
        </p>
      </div>

      <!-- Playback controls -->
      <div class="flex items-center gap-2">
        <button
          class="rounded-lg border border-forest-200 px-3 py-2 text-sm text-forest-700 hover:bg-forest-50 transition-colors disabled:opacity-40"
          :disabled="atEnd"
          @click="togglePlay"
        >
          {{ playing ? '❚❚ Pause' : '▶ Lecture' }}
        </button>
        <div class="flex rounded-lg border border-forest-200 overflow-hidden text-sm">
          <button
            v-for="s in [1, 2, 4]"
            :key="s"
            class="px-2.5 py-2 transition-colors"
            :class="speed === s ? 'bg-forest-100 text-forest-700 font-600' : 'text-ink-500 hover:bg-forest-50'"
            @click="setSpeed(s)"
          >
            {{ s }}×
          </button>
        </div>
        <button
          class="rounded-lg border border-forest-200 px-3 py-2 text-sm text-ink-500 hover:bg-forest-50 transition-colors disabled:opacity-40"
          :disabled="atEnd"
          @click="skipToEnd"
        >
          ⏭ Fin
        </button>
        <button
          class="rounded-lg border border-forest-200 px-3 py-2 text-sm text-ink-500 hover:bg-forest-50 transition-colors"
          @click="restart"
        >
          ⟲
        </button>
      </div>
    </div>

    <!-- Progress -->
    <div class="mb-6 h-1 rounded-full bg-forest-100 overflow-hidden">
      <div class="h-full bg-forest-500 transition-all duration-200" :style="{ width: `${progressPct}%` }" />
    </div>

    <div class="grid lg:grid-cols-5 gap-5">
      <div class="lg:col-span-3 space-y-5">
        <ContextWindowGauge
          :by-category="snapshot.byCategory"
          :total="snapshot.total"
          :max="timeline.maxTokens"
          :input-tokens="snapshot.inputTokens"
          :output-tokens="snapshot.outputTokens"
        />
        <SubagentWindow :subagent-tokens="snapshot.subagentTokens" :max="timeline.maxTokens" />
        <EventTimeline :events="visibleEvents" />
      </div>

      <div class="lg:col-span-2 space-y-5">
        <CategoryLegend :by-category="snapshot.byCategory" />

        <div class="rounded-2xl border border-forest-200 bg-forest-50/50 p-5">
          <p class="text-sm text-ink-600 leading-relaxed">
            Les tokens d'entrée et de sortie sont déduits de l'<strong>occupation finale</strong> de
            la fenêtre (facturation « A »). Ils alimentent les critères
            <strong>Économique</strong> et <strong>Environnemental</strong>.
          </p>
        </div>

        <transition name="fade">
          <button
            v-if="atEnd"
            class="w-full inline-flex items-center justify-center gap-2 rounded-xl bg-forest-600 px-6 py-3.5 font-600 text-paper shadow-sm hover:bg-forest-700 transition-colors animate-rise"
            @click="goToResult"
          >
            Voir l'évaluation
            <span aria-hidden="true">→</span>
          </button>
        </transition>
      </div>
    </div>
  </div>
</template>

<style scoped>
.fade-enter-active {
  transition: opacity 0.3s ease;
}
.fade-enter-from {
  opacity: 0;
}
</style>
