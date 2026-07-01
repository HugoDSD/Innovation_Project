<script setup lang="ts">
import { RouterLink } from 'vue-router'
import { useHistory } from '@/stores/history'
import { getModel, getProvider } from '@/data/catalog'
import { formatDate, formatInt } from '@/lib/format'
import { VERDICT_STYLES } from '@/lib/labels'

const { history, remove, clear } = useHistory()
</script>

<template>
  <div class="mx-auto max-w-4xl px-5 py-10 animate-rise">
    <div class="mb-6 flex flex-wrap items-end justify-between gap-3">
      <div>
        <h1 class="text-3xl font-700 text-ink-900">Historique</h1>
        <p class="mt-1 text-ink-500">
          {{ history.length }} évaluation{{ history.length > 1 ? 's' : '' }} enregistrée{{
            history.length > 1 ? 's' : ''
          }}
          localement.
        </p>
      </div>
      <div class="flex gap-2">
        <RouterLink
          to="/"
          class="rounded-xl bg-forest-600 px-4 py-2 text-sm font-600 text-paper hover:bg-forest-700 transition-colors"
        >
          + Nouvelle évaluation
        </RouterLink>
        <button
          v-if="history.length"
          class="rounded-xl border border-forest-200 px-4 py-2 text-sm text-ink-500 hover:bg-forest-50 transition-colors"
          @click="clear"
        >
          Tout effacer
        </button>
      </div>
    </div>

    <div v-if="!history.length" class="rounded-2xl border border-dashed border-forest-200 p-12 text-center">
      <p class="text-ink-500">Aucune évaluation pour l'instant.</p>
      <RouterLink to="/" class="mt-3 inline-block text-forest-600 font-600 hover:underline">
        Évaluer un premier workflow →
      </RouterLink>
    </div>

    <ul v-else class="space-y-3">
      <li v-for="ev in history" :key="ev.id">
        <RouterLink
          :to="{ name: 'history-detail', params: { id: ev.id } }"
          class="group flex items-center gap-4 rounded-2xl border border-forest-200 bg-white/70 p-4 shadow-sm hover:border-forest-400 hover:shadow transition-all"
        >
          <span
            class="flex h-10 w-10 shrink-0 items-center justify-center rounded-xl text-lg font-700"
            :style="{
              backgroundColor: VERDICT_STYLES[ev.verdict.level].color,
              color: 'white',
            }"
          >
            {{ VERDICT_STYLES[ev.verdict.level].icon }}
          </span>

          <div class="min-w-0 flex-1">
            <p class="text-sm text-ink-800 truncate group-hover:text-forest-700 transition-colors">
              {{ ev.input.workflowDescription }}
            </p>
            <p class="text-xs text-ink-400">
              {{ formatDate(ev.createdAt) }} · {{ getModel(ev.input.aiModelId).name }} ·
              {{ getProvider(ev.input.cloudProviderId).name }} ·
              {{ formatInt(ev.input.runFrequency) }} exéc./mois
            </p>
          </div>

          <span
            class="hidden sm:inline shrink-0 text-sm font-600"
            :style="{ color: VERDICT_STYLES[ev.verdict.level].color }"
          >
            {{ ev.verdict.level }}
          </span>

          <button
            class="shrink-0 rounded-lg p-1.5 text-ink-400 hover:text-verdict-bad hover:bg-paper-100 transition-colors"
            title="Supprimer"
            @click.prevent="remove(ev.id)"
          >
            ✕
          </button>
        </RouterLink>
      </li>
    </ul>
  </div>
</template>
