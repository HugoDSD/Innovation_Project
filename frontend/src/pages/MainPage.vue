<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import ResultsSection from '../components/ResultsSection.vue'
import { calculateImpact } from '../services/evaluation.js'

const router = useRouter()
const currentStep = ref(1)
const loading = ref(false)
const error = ref('')
const results = ref(null)

const step1 = ref({
  projectName: '',
  projectDescription: '',
  hoursSavedReports: 0,
  hoursSavedImages: 0,
  hoursSavedPresentations: 0,
  dataSensitivity: 1,
  legalRisk: 1
})

const step2 = ref({
  modelName: '',
  provider: '',
  inputTokens: 0,
  outputTokens: 0
})

const AI_MODELS = ['GPT OSS 20B', 'GPT OSS 120B', 'DeepSeek V3.1', 'DeepSeek R1']
const PROVIDERS = ['Microsoft', 'Amazon', 'Référence']

const validateStep1 = () =>
  step1.value.projectName.trim() !== '' &&
  step1.value.dataSensitivity >= 1 &&
  step1.value.legalRisk >= 1

const validateStep2 = () =>
  step2.value.modelName !== '' &&
  step2.value.provider !== '' &&
  step2.value.inputTokens > 0 &&
  step2.value.outputTokens > 0

const goToStep2 = () => {
  if (validateStep1()) currentStep.value = 2
}

const handleSubmit = async () => {
  if (!validateStep2()) return
  loading.value = true
  error.value = ''
  try {
    const payload = {
      modelName: step2.value.modelName,
      provider: step2.value.provider,
      inputTokens: step2.value.inputTokens,
      outputTokens: step2.value.outputTokens,
      hoursSavedReports: step1.value.hoursSavedReports,
      hoursSavedImages: step1.value.hoursSavedImages,
      hoursSavedPresentations: step1.value.hoursSavedPresentations,
      dataSensitivity: step1.value.dataSensitivity,
      legalRisk: step1.value.legalRisk
    }
    results.value = await calculateImpact(payload)
    setTimeout(() => {
      document.querySelector('.results-section')?.scrollIntoView({ behavior: 'smooth' })
    }, 100)
  } catch (err) {
    if (err.status === 401) {
      localStorage.removeItem('token')
      router.push('/login')
    } else {
      error.value = err.message ?? 'Une erreur est survenue lors de l\'analyse.'
    }
  } finally {
    loading.value = false
  }
}

const handleLogout = () => {
  localStorage.removeItem('token')
  router.push('/login')
}

const reset = () => {
  currentStep.value = 1
  results.value = null
  error.value = ''
  step1.value = { projectName: '', projectDescription: '', hoursSavedReports: 0, hoursSavedImages: 0, hoursSavedPresentations: 0, dataSensitivity: 1, legalRisk: 1 }
  step2.value = { modelName: '', provider: '', inputTokens: 0, outputTokens: 0 }
}
</script>

<template>
  <div class="main-container">
    <header class="header">
      <div class="header-content">
        <h1>Évaluateur d'Impact IA</h1>
        <div class="header-actions">
          <button class="nav-btn" @click="router.push('/history')">Historique</button>
          <button class="logout-btn" @click="handleLogout">Déconnexion</button>
        </div>
      </div>
    </header>

    <div class="content">
      <div class="form-container">
        <div class="step-indicator">
          <span :class="{ active: currentStep === 1 }">1. Contexte du projet</span>
          <span class="divider">›</span>
          <span :class="{ active: currentStep === 2 }">2. Configuration IA</span>
        </div>

        <!-- Step 1 -->
        <form v-if="currentStep === 1" @submit.prevent="goToStep2" class="project-form">
          <h2 class="form-title">Décrivez votre projet</h2>

          <div class="form-group">
            <label for="projectName">Nom du projet *</label>
            <input id="projectName" v-model="step1.projectName" type="text"
              placeholder="Ex: Automatisation des rapports RH" required>
          </div>

          <div class="form-group">
            <label for="projectDescription">Description</label>
            <textarea id="projectDescription" v-model="step1.projectDescription" rows="3"
              placeholder="Décrivez votre projet..."></textarea>
          </div>

          <h3 class="section-title">Gain de temps estimé (heures / semaine)</h3>

          <div class="form-row">
            <div class="form-group">
              <label for="hoursSavedReports">Rapports</label>
              <input id="hoursSavedReports" v-model.number="step1.hoursSavedReports"
                type="number" min="0" step="0.5">
            </div>
            <div class="form-group">
              <label for="hoursSavedImages">Images</label>
              <input id="hoursSavedImages" v-model.number="step1.hoursSavedImages"
                type="number" min="0" step="0.5">
            </div>
            <div class="form-group">
              <label for="hoursSavedPresentations">Présentations</label>
              <input id="hoursSavedPresentations" v-model.number="step1.hoursSavedPresentations"
                type="number" min="0" step="0.5">
            </div>
          </div>

          <h3 class="section-title">Profil de risque</h3>

          <div class="form-row">
            <div class="form-group">
              <label for="dataSensitivity">Sensibilité des données (1–5) *</label>
              <input id="dataSensitivity" v-model.number="step1.dataSensitivity"
                type="number" min="1" max="5" required>
            </div>
            <div class="form-group">
              <label for="legalRisk">Risque légal (1–5) *</label>
              <input id="legalRisk" v-model.number="step1.legalRisk"
                type="number" min="1" max="5" required>
            </div>
          </div>

          <div class="form-actions">
            <button type="submit" class="submit-btn">Suivant →</button>
          </div>
        </form>

        <!-- Step 2 -->
        <form v-if="currentStep === 2" @submit.prevent="handleSubmit" class="project-form">
          <h2 class="form-title">Configurez l'IA</h2>

          <div class="form-row">
            <div class="form-group">
              <label for="modelName">Modèle IA *</label>
              <select id="modelName" v-model="step2.modelName" required>
                <option value="">Sélectionnez un modèle...</option>
                <option v-for="model in AI_MODELS" :key="model" :value="model">{{ model }}</option>
              </select>
            </div>
            <div class="form-group">
              <label for="provider">Fournisseur *</label>
              <select id="provider" v-model="step2.provider" required>
                <option value="">Sélectionnez un fournisseur...</option>
                <option v-for="p in PROVIDERS" :key="p" :value="p">{{ p }}</option>
              </select>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="inputTokens">Tokens en entrée *</label>
              <input id="inputTokens" v-model.number="step2.inputTokens"
                type="number" min="1" required>
            </div>
            <div class="form-group">
              <label for="outputTokens">Tokens en sortie *</label>
              <input id="outputTokens" v-model.number="step2.outputTokens"
                type="number" min="1" required>
            </div>
          </div>

          <p v-if="error" class="error-message">{{ error }}</p>

          <div class="form-actions">
            <button type="button" class="reset-btn" @click="currentStep = 1">← Retour</button>
            <button type="submit" class="submit-btn" :disabled="loading">
              {{ loading ? 'Analyse en cours...' : 'Analyser' }}
            </button>
            <button type="button" class="reset-btn" @click="reset">Réinitialiser</button>
          </div>
        </form>
      </div>

      <ResultsSection
        v-if="results"
        :results="results"
        class="results-section"
      />
    </div>
  </div>
</template>

<style scoped>
.main-container {
  min-height: 100vh;
  background-color: lightgrey;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.header {
  background-color: darkblue;
  color: white;
  padding: 1.5rem 0;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-content {
  max-width: 900px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 1.5rem;
}

.header h1 {
  margin: 0;
  font-size: 1.8rem;
}

.header-actions {
  display: flex;
  gap: 0.75rem;
}

.nav-btn {
  background: transparent;
  color: white;
  border: 2px solid white;
  padding: 0.5rem 1rem;
  border-radius: 5px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s;
}

.logout-btn {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 2px solid white;
  padding: 0.5rem 1rem;
  border-radius: 5px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s;
}

.logout-btn:hover {
  background: rgba(255, 255, 255, 0.3);
}

.content {
  max-width: 1000px;
  margin: 2rem auto;
  padding: 0 1.5rem;
}

.form-container {
  background: white;
  padding: 2rem;
  border-radius: 10px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1);
}

.step-indicator {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  margin-bottom: 1.5rem;
  font-size: 0.95rem;
  color: #999;
}

.step-indicator span.active {
  color: darkblue;
  font-weight: 600;
}

.step-indicator .divider {
  color: #ccc;
}

.form-title {
  color: #333;
  font-size: 1.6rem;
  margin-top: 0;
  margin-bottom: 0.5rem;
  text-align: center;
}

.form-subtitle {
  text-align: center;
  color: #666;
  margin-bottom: 1.5rem;
}

.section-title {
  font-size: 1rem;
  color: #333;
  margin: 0.5rem 0;
  font-weight: 600;
}

.project-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group.full-width {
  grid-column: 1 / -1;
}

label {
  margin-bottom: 0.5rem;
  color: #333;
  font-weight: 500;
  font-size: 0.95rem;
}

input,
select,
textarea {
  padding: 0.75rem;
  border: 2px solid #ddd;
  border-radius: 5px;
  font-size: 1rem;
  font-family: inherit;
  transition: border-color 0.3s;
}

input:focus,
select:focus,
textarea:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

textarea {
  resize: vertical;
}

.error-message {
  color: #e74c3c;
  font-size: 0.9rem;
  margin: 0;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
  margin-top: 1rem;
}

.submit-btn {
  padding: 0.875rem 2rem;
  background-color: darkblue;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.submit-btn:hover {
  box-shadow: 0 5px 20px rgba(102, 126, 234, 0.4);
}

.submit-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.reset-btn {
  padding: 0.875rem 2rem;
  background: #f0f0f0;
  color: #333;
  border: 2px solid #ddd;
  border-radius: 5px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.reset-btn:hover {
  background: #e8e8e8;
  border-color: #999;
}

@media (max-width: 600px) {
  .form-row {
    grid-template-columns: 1fr;
  }

  .form-actions {
    flex-direction: column;
  }

  .submit-btn,
  .reset-btn {
    width: 100%;
  }

  .header-content {
    flex-direction: column;
    gap: 1rem;
  }

  .header h1 {
    font-size: 1.5rem;
  }
}
</style>
