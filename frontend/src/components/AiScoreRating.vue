<script setup>
import { ref } from 'vue'
import { setAiScore } from '../services/evaluation.js'

const props = defineProps({
  evaluationId: { type: Number, required: true }
})

const SCORES = ['Utile', 'Moyen', 'Non utile', 'Mieux sans IA']
const selected = ref(null)
const submitted = ref(false)
const loading = ref(false)
const error = ref('')

const submitScore = async () => {
  if (!selected.value) return
  loading.value = true
  error.value = ''
  try {
    await setAiScore(props.evaluationId, selected.value)
    submitted.value = true
  } catch {
    error.value = 'Erreur lors de l\'enregistrement.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="rating-container">
    <h3 class="rating-title">Selon vous, l'IA est-elle utile pour ce projet ?</h3>

    <div v-if="!submitted" class="rating-options">
      <button
        v-for="score in SCORES"
        :key="score"
        class="score-btn"
        :class="{ selected: selected === score }"
        type="button"
        @click="selected = score"
      >
        {{ score }}
      </button>
      <button
        class="submit-rating-btn"
        type="button"
        :disabled="!selected || loading"
        @click="submitScore"
      >
        {{ loading ? 'Enregistrement...' : 'Confirmer' }}
      </button>
      <p v-if="error" class="error-message">{{ error }}</p>
    </div>

    <p v-else class="confirmation">✓ Évaluation enregistrée : <strong>{{ selected }}</strong></p>
  </div>
</template>

<style scoped>
.rating-container {
  margin-top: 2rem;
  padding: 1.5rem;
  background: #f8f9fa;
  border-radius: 8px;
  border-left: 4px solid #667eea;
}

.rating-title { margin: 0 0 1rem; color: #333; font-size: 1rem; font-weight: 600; }

.rating-options { display: flex; flex-wrap: wrap; gap: 0.75rem; align-items: center; }

.score-btn {
  padding: 0.6rem 1.2rem;
  border: 2px solid #ddd;
  border-radius: 20px;
  background: white;
  cursor: pointer;
  font-size: 0.9rem;
  transition: all 0.2s;
}

.score-btn.selected { border-color: darkblue; background: darkblue; color: white; }

.submit-rating-btn {
  padding: 0.6rem 1.5rem;
  background: darkblue;
  color: white;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 0.9rem;
  margin-left: auto;
}

.submit-rating-btn:disabled { opacity: 0.5; cursor: not-allowed; }

.confirmation { color: #155724; margin: 0; }

.error-message { color: #e74c3c; font-size: 0.9rem; width: 100%; margin: 0; }
</style>
