<script setup lang="ts">
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import type { Evaluation } from '@/types'
import { useEvaluation } from '@/stores/evaluation'
import { useHistory } from '@/stores/history'
import { getModel, getProvider } from '@/data/catalog'
import { formatEur, formatInt, formatNumber, formatUsd, formatTokens } from '@/lib/format'
import { CRITERION_KIND, CRITERION_LABELS } from '@/lib/labels'
import { rateLowerBetter, THRESHOLDS, costToValueRatio } from '@/scoring/ratings'

function rateEnv(value: number, axis: 'co2' | 'water'): number {
  return rateLowerBetter(
    value,
    axis === 'co2' ? THRESHOLDS.environmentalCo2Kg : THRESHOLDS.environmentalWaterL,
  )
}
import VerdictBanner from '@/components/result/VerdictBanner.vue'
import RadarChart from '@/components/result/RadarChart.vue'
import CriterionCard from '@/components/result/CriterionCard.vue'

const props = defineProps<{ id?: string }>()

const router = useRouter()
const { state } = useEvaluation()
const { get } = useHistory()

// Either a saved evaluation (history detail) or the freshly finalized one.
const evaluation = computed<Evaluation | undefined>(() =>
  props.id ? get(props.id) : (state.result ?? undefined),
)

const model = computed(() => (evaluation.value ? getModel(evaluation.value.input.aiModelId) : null))
const provider = computed(() =>
  evaluation.value ? getProvider(evaluation.value.input.cloudProviderId) : null,
)

const efficiencyMetrics = computed(() => {
  const e = evaluation.value!.variables.efficiency
  const input = evaluation.value!.input
  return [
    { label: 'Heures économisées / exéc.', value: `${formatNumber(e.hoursSavedPerRun)} h` },
    { label: 'Fraction du temps économisée', value: `${Math.round(e.aiSavingsFraction * 100)} %` },
    { label: 'Taux horaire', value: `${formatEur(e.hourlyRate)}/h (${input.experienceLevel})` },
    { label: 'Valeur économisée (total)', value: formatEur(e.valueSaved) },
  ]
})

const environmentalMetrics = computed(() => {
  const v = evaluation.value!.variables.environmental
  const co2Rating = rateEnv(v.co2Kg, 'co2')
  const waterRating = rateEnv(v.waterL, 'water')
  return [
    { label: 'Énergie (total)', value: `${formatNumber(v.energyKwh, 3)} kWh` },
    { label: 'CO₂ émis (total)', value: `${formatNumber(v.co2Kg, 3)} kg · ${co2Rating}/5` },
    { label: 'Eau consommée (total)', value: `${formatNumber(v.waterL, 2)} L · ${waterRating}/5` },
  ]
})

const economicMetrics = computed(() => {
  const vars = evaluation.value!.variables
  const ratio = costToValueRatio(vars)
  const ratioDisplay = isFinite(ratio) ? `${formatNumber(ratio, 3)} $/€` : '—'
  return [
    { label: 'Coût / exécution', value: formatUsd(vars.economic.costUsdPerRun) },
    { label: 'Coût total', value: formatUsd(vars.economic.costUsdTotal) },
    { label: 'Ratio coût / valeur', value: ratioDisplay },
  ]
})

const riskMetrics = computed(() => {
  const v = evaluation.value!.variables.risk
  return [
    { label: 'Sensibilité des données', value: v.dataSensitivity },
    { label: 'Risque juridique', value: v.legalRisk },
  ]
})

function newEvaluation() {
  router.push({ name: 'form' })
}
</script>

<template>
  <div v-if="!evaluation" class="mx-auto max-w-xl px-5 py-20 text-center">
    <p class="text-ink-500">Aucune évaluation à afficher.</p>
    <button
      class="mt-4 rounded-xl bg-forest-600 px-5 py-2.5 font-600 text-paper hover:bg-forest-700 transition-colors"
      @click="newEvaluation"
    >
      Lancer une évaluation
    </button>
  </div>

  <div v-else class="mx-auto max-w-5xl px-5 py-10 animate-rise">
    <div class="mb-6 flex flex-wrap items-end justify-between gap-3">
      <div>
        <p class="text-sm font-600 uppercase tracking-wider text-forest-500 mb-2">Étape 3 · Résultat</p>
        <h1 class="text-3xl font-700 text-ink-900">Évaluation du workflow</h1>
      </div>
      <button
        class="rounded-xl border border-forest-300 px-4 py-2 text-sm font-600 text-forest-700 hover:bg-forest-50 transition-colors"
        @click="newEvaluation"
      >
        + Nouvelle évaluation
      </button>
    </div>

    <!-- Workflow recap -->
    <div class="mb-6 rounded-2xl border border-forest-200 bg-paper-100/50 p-4 text-sm text-ink-600">
      <p class="italic">« {{ evaluation.input.workflowDescription }} »</p>
      <p class="mt-2 text-xs text-ink-400">
        {{ model?.name }} · {{ provider?.name }} ({{ provider?.region }}) ·
        {{ formatInt(evaluation.input.runFrequency) }} exéc./mois
      </p>
    </div>

    <div class="grid lg:grid-cols-5 gap-6">
      <!-- Verdict + radar -->
      <div class="lg:col-span-2 space-y-6">
        <VerdictBanner :verdict="evaluation.verdict" />
        <div class="rounded-2xl border border-forest-200 bg-white/70 p-5 shadow-sm flex justify-center">
          <RadarChart :ratings="evaluation.ratings" />
        </div>
      </div>

      <!-- Criteria detail -->
      <div class="lg:col-span-3 grid sm:grid-cols-2 gap-4">
        <CriterionCard
          :title="CRITERION_LABELS.efficiency"
          :kind="CRITERION_KIND.efficiency"
          :rating="evaluation.ratings.efficiency"
          :metrics="efficiencyMetrics"
        />
        <CriterionCard
          :title="CRITERION_LABELS.environmental"
          :kind="CRITERION_KIND.environmental"
          :rating="evaluation.ratings.environmental"
          :metrics="environmentalMetrics"
        />
        <CriterionCard
          :title="CRITERION_LABELS.economic"
          :kind="CRITERION_KIND.economic"
          :rating="evaluation.ratings.economic"
          :metrics="economicMetrics"
        />
        <CriterionCard
          :title="CRITERION_LABELS.risk"
          :kind="CRITERION_KIND.risk"
          :rating="evaluation.ratings.risk"
          :metrics="riskMetrics"
        />
      </div>
    </div>

    <!-- Session summary -->
    <div class="mt-6 rounded-2xl border border-forest-200 bg-white/70 p-5 shadow-sm">
      <p class="text-xs uppercase tracking-wider text-ink-400 font-600 mb-3">Session simulée</p>
      <div class="grid grid-cols-2 sm:grid-cols-4 gap-4 text-sm">
        <div>
          <p class="text-ink-400 text-xs">Tokens d'entrée</p>
          <p class="font-display text-xl font-600 text-ink-800 tabular-nums">
            {{ formatTokens(evaluation.variables.tokens.inputTokens) }}
          </p>
        </div>
        <div>
          <p class="text-ink-400 text-xs">Tokens de sortie</p>
          <p class="font-display text-xl font-600 text-ink-800 tabular-nums">
            {{ formatTokens(evaluation.variables.tokens.outputTokens) }}
          </p>
        </div>
        <div>
          <p class="text-ink-400 text-xs">Tours</p>
          <p class="font-display text-xl font-600 text-ink-800 tabular-nums">
            {{ evaluation.variables.tokens.turns }}
          </p>
        </div>
        <div>
          <p class="text-ink-400 text-xs">Fenêtre du modèle</p>
          <p class="font-display text-xl font-600 text-ink-800 tabular-nums">
            {{ formatInt(evaluation.timeline.maxTokens) }}
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
