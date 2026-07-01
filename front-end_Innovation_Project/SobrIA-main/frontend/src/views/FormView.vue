<script setup lang="ts">
import { reactive, computed } from 'vue'
import { useRouter } from 'vue-router'
import { AI_MODELS, CLOUD_PROVIDERS, EXPERIENCE_LEVELS } from '@/data/catalog'
import { startEvaluation } from '@/stores/evaluation'
import { formatInt } from '@/lib/format'
import type { WorkflowInput } from '@/types'

const router = useRouter()

const form = reactive<WorkflowInput>({
  workflowDescription: '',
  runFrequency: 100,
  employeeCount: 1,
  hoursPerRun: 1,
  experienceLevel: 'confirmé',
  aiModelId: AI_MODELS[1]!.id,
  cloudProviderId: CLOUD_PROVIDERS[0]!.id,
})

const examples = [
  'Traduire à la volée les fiches produit du site e-commerce dans 6 langues, au lieu d\'une agence de traduction.',
  'Trier et résumer les tickets entrants du support client avant qu\'un agent les prenne en charge.',
  'Générer automatiquement les descriptions de produits à partir de leurs caractéristiques techniques.',
]

const descriptionValid = computed(() => form.workflowDescription.trim().length >= 20)
const frequencyValid = computed(() => form.runFrequency > 0)
const employeeValid = computed(() => form.employeeCount >= 1)
const hoursValid = computed(() => form.hoursPerRun > 0)
const canSubmit = computed(
  () => descriptionValid.value && frequencyValid.value && employeeValid.value && hoursValid.value,
)

const selectedModel = computed(() => AI_MODELS.find((m) => m.id === form.aiModelId)!)
const selectedLevel = computed(() => EXPERIENCE_LEVELS.find((l) => l.value === form.experienceLevel)!)

function useExample(text: string) {
  form.workflowDescription = text
}

function submit() {
  if (!canSubmit.value) return
  startEvaluation({ ...form })
  router.push({ name: 'simulation' })
}
</script>

<template>
  <div class="mx-auto max-w-3xl px-5 py-10 animate-rise">
    <div class="mb-8">
      <p class="text-sm font-600 uppercase tracking-wider text-forest-500 mb-2">Étape 1 · Workflow</p>
      <h1 class="text-3xl sm:text-4xl font-700 text-ink-900 mb-3">
        Décrivez le workflow à évaluer
      </h1>
      <p class="text-ink-500 leading-relaxed">
        SobrIA estime si confier cette tâche à l'IA en vaut la peine. Vous ne saisissez aucun
        chiffre de tokens — ils seront déduits de votre description à l'étape suivante.
      </p>
    </div>

    <form class="space-y-7" @submit.prevent="submit">
      <!-- Description -->
      <div>
        <label for="desc" class="block text-sm font-600 text-ink-700 mb-1.5">
          Description de la tâche <span class="text-forest-500">et de ce qu'elle remplace</span>
        </label>
        <textarea
          id="desc"
          v-model="form.workflowDescription"
          rows="4"
          placeholder="Ex. Résumer chaque réunion d'équipe à partir de sa transcription, au lieu d'une prise de notes manuelle…"
          class="w-full rounded-xl border border-forest-200 bg-white/70 px-4 py-3 text-ink-900 placeholder:text-ink-400 focus:border-forest-500 focus:ring-2 focus:ring-forest-300/50 outline-none transition resize-y"
        />
        <div class="mt-2 flex flex-wrap items-center gap-2">
          <span class="text-xs text-ink-400">Exemples :</span>
          <button
            v-for="(ex, i) in examples"
            :key="i"
            type="button"
            class="text-xs px-2.5 py-1 rounded-full border border-forest-200 text-forest-600 hover:bg-forest-50 transition-colors"
            @click="useExample(ex)"
          >
            Exemple {{ i + 1 }}
          </button>
        </div>
        <p v-if="form.workflowDescription && !descriptionValid" class="mt-1.5 text-xs text-verdict-bad">
          Décrivez la tâche un peu plus précisément (au moins 20 caractères).
        </p>
      </div>

      <!-- Workforce baseline -->
      <div>
        <p class="block text-sm font-600 text-ink-700 mb-3">
          Actuellement, sans IA, ce workflow mobilise…
        </p>
        <div class="grid sm:grid-cols-3 gap-4">
          <!-- Employee count -->
          <div>
            <label for="emp" class="block text-xs text-ink-500 mb-1.5">Nombre d'employés</label>
            <input
              id="emp"
              v-model.number="form.employeeCount"
              type="number"
              min="1"
              max="500"
              class="w-full rounded-xl border border-forest-200 bg-white/70 px-4 py-2.5 text-ink-900 focus:border-forest-500 focus:ring-2 focus:ring-forest-300/50 outline-none transition"
            />
          </div>

          <!-- Hours per run -->
          <div>
            <label for="hours" class="block text-xs text-ink-500 mb-1.5">Heures par exécution</label>
            <input
              id="hours"
              v-model.number="form.hoursPerRun"
              type="number"
              min="0.1"
              step="0.5"
              class="w-full rounded-xl border border-forest-200 bg-white/70 px-4 py-2.5 text-ink-900 focus:border-forest-500 focus:ring-2 focus:ring-forest-300/50 outline-none transition"
            />
          </div>

          <!-- Experience level -->
          <div>
            <label for="level" class="block text-xs text-ink-500 mb-1.5">Niveau d'expérience</label>
            <select
              id="level"
              v-model="form.experienceLevel"
              class="w-full rounded-xl border border-forest-200 bg-white/70 px-4 py-2.5 text-ink-900 focus:border-forest-500 focus:ring-2 focus:ring-forest-300/50 outline-none transition"
            >
              <option v-for="l in EXPERIENCE_LEVELS" :key="l.value" :value="l.value">
                {{ l.label }}
              </option>
            </select>
            <p class="mt-1 text-xs text-ink-400">{{ selectedLevel.hint }}</p>
          </div>
        </div>
      </div>

      <!-- Frequency -->
      <div>
        <label for="freq" class="block text-sm font-600 text-ink-700 mb-1.5">
          Fréquence d'exécution
        </label>
        <div class="flex items-center gap-3">
          <input
            id="freq"
            v-model.number="form.runFrequency"
            type="number"
            min="1"
            class="w-32 rounded-xl border border-forest-200 bg-white/70 px-4 py-2.5 text-ink-900 focus:border-forest-500 focus:ring-2 focus:ring-forest-300/50 outline-none transition"
          />
          <span class="text-sm text-ink-500">exécutions / mois</span>
        </div>
        <input
          v-model.number="form.runFrequency"
          type="range"
          min="1"
          max="5000"
          step="1"
          class="mt-3 w-full accent-forest-600"
        />
        <p class="text-xs text-ink-400">
          {{ formatInt(form.runFrequency) }} runs/mois
        </p>
      </div>

      <!-- Model + provider -->
      <div class="grid sm:grid-cols-2 gap-5">
        <div>
          <label for="model" class="block text-sm font-600 text-ink-700 mb-1.5">Modèle d'IA</label>
          <select
            id="model"
            v-model="form.aiModelId"
            class="w-full rounded-xl border border-forest-200 bg-white/70 px-4 py-2.5 text-ink-900 focus:border-forest-500 focus:ring-2 focus:ring-forest-300/50 outline-none transition"
          >
            <option v-for="m in AI_MODELS" :key="m.id" :value="m.id">
              {{ m.name }} · {{ m.vendor }}
            </option>
          </select>
          <p class="mt-1.5 text-xs text-ink-400">
            Fenêtre de contexte : {{ formatInt(selectedModel.contextWindow) }} tokens
          </p>
        </div>

        <div>
          <label for="provider" class="block text-sm font-600 text-ink-700 mb-1.5">Fournisseur cloud</label>
          <select
            id="provider"
            v-model="form.cloudProviderId"
            class="w-full rounded-xl border border-forest-200 bg-white/70 px-4 py-2.5 text-ink-900 focus:border-forest-500 focus:ring-2 focus:ring-forest-300/50 outline-none transition"
          >
            <option v-for="p in CLOUD_PROVIDERS" :key="p.id" :value="p.id">
              {{ p.name }} — {{ p.region }}
            </option>
          </select>
          <p class="mt-1.5 text-xs text-ink-400">Détermine les facteurs carbone, eau et prix</p>
        </div>
      </div>

      <div class="pt-2 flex items-center gap-4">
        <button
          type="submit"
          :disabled="!canSubmit"
          class="inline-flex items-center gap-2 rounded-xl bg-forest-600 px-6 py-3 font-600 text-paper shadow-sm hover:bg-forest-700 disabled:opacity-40 disabled:cursor-not-allowed transition-colors"
        >
          Simuler la session
          <span aria-hidden="true">→</span>
        </button>
        <span class="text-xs text-ink-400">Étape suivante : simulation de la fenêtre de contexte</span>
      </div>
    </form>
  </div>
</template>
