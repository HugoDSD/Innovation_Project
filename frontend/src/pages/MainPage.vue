<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import apiService from '../services/apiService'

const router = useRouter()

// Sidebar
const sidebarOpen = ref(false)

const MOCK_RESULT = {
  isApproved: false,
  evaluationId: 7,
  message: "REJETÉ : Les bénéfices (gain de temps) ne compensent pas les risques ou l'impact.",
  totalEnergyKwh: 0.007417222,
  totalCarbonKg: 0.0005941194822,
  totalWaterLiters: 0.00623046648,
  totalCostUsd: 0.004900000000000001,
  totalHoursSaved: 6.5,
  riskScore: 4.5
}

const MOCK_HISTORY = [
  {
    id: '6', modelName: 'DeepSeek V3.1', aiScore: '',
    carbonFootprint: 0.0005941194822, waterFootprintLiters: 0.00623046648,
    energyKwh: 0.007417222, costUsd: 0.004900000000000001,
    hoursSaved: 6.5, riskScore: 4.5, isApproved: false,
    createdAt: '2026-06-22T22:44:20.532997Z'
  },
  {
    id: '5', modelName: 'DeepSeek V3.1', aiScore: 'Utile',
    carbonFootprint: 0.0005941194822, waterFootprintLiters: 0.00623046648,
    energyKwh: 0.007417222, costUsd: 0.004900000000000001,
    hoursSaved: 6.5, riskScore: 4.5, isApproved: false,
    createdAt: '2026-06-22T20:52:54.484802Z'
  }
]

// Evaluation form
const formData = ref({
  modelName: 'DeepSeek V3.1',
  provider: 'Référence',
  inputTokens: '5000',
  outputTokens: '15000',
  hoursSavedReports: '4.5',
  hoursSavedImages: '0',
  hoursSavedPresentations: '2',
  dataSensitivity: 4,
  legalRisk: 5
})
const results = ref(null)
const showResults = ref(false)
const loading = ref(false)
const formError = ref('')

// AI Score form
const scoreForm = ref({ evaluationId: '', aiScore: '' })
const scoreResult = ref(null)
const scoreError = ref('')
const scoreLoading = ref(false)

// History
const historyFilters = ref({ minCarbon: '', maxCarbon: '', aiScore: '', startDate: '', endDate: '' })
const historyItems = ref([])
const historyLoading = ref(false)
const historyError = ref('')
const historySearched = ref(false)

const handleSubmit = async () => {
  formError.value = ''
  const d = formData.value
  if (!d.modelName || !d.provider || d.inputTokens === '' || d.outputTokens === '') {
    formError.value = 'Veuillez remplir les champs obligatoires (modèle, fournisseur, tokens)'
    return
  }
  loading.value = true
  try {
    let data
    if (apiService.token === 'test-token-local') {
      await new Promise(r => setTimeout(r, 600))
      data = { ...MOCK_RESULT }
    } else {
      const payload = {
        modelName: d.modelName,
        provider: d.provider,
        inputTokens: Number(d.inputTokens),
        outputTokens: Number(d.outputTokens),
        hoursSavedReports: Number(d.hoursSavedReports) || 0,
        hoursSavedImages: Number(d.hoursSavedImages) || 0,
        hoursSavedPresentations: Number(d.hoursSavedPresentations) || 0,
        dataSensitivity: Number(d.dataSensitivity),
        legalRisk: Number(d.legalRisk)
      }
      data = await apiService.calculateEvaluation(payload)
    }
    results.value = data
    showResults.value = true
    if (data.evaluationId != null) {
      scoreForm.value.evaluationId = String(data.evaluationId)
    }
    setTimeout(() => {
      document.querySelector('.results-section')?.scrollIntoView({ behavior: 'smooth' })
    }, 100)
  } catch (e) {
    formError.value = e.message || 'Erreur lors du calcul'
  } finally {
    loading.value = false
  }
}

const handleScoreSubmit = async () => {
  scoreError.value = ''
  scoreResult.value = null
  if (!scoreForm.value.evaluationId || !scoreForm.value.aiScore) {
    scoreError.value = "Veuillez remplir l'ID et la note"
    return
  }
  scoreLoading.value = true
  try {
    const data = await apiService.updateEvaluationScore(scoreForm.value.evaluationId, scoreForm.value.aiScore)
    scoreResult.value = data
    if (results.value && String(results.value.evaluationId) === String(scoreForm.value.evaluationId)) {
      results.value = { ...results.value, isApproved: data.isApproved }
    }
  } catch (e) {
    scoreError.value = e.message || 'Erreur lors de la notation'
  } finally {
    scoreLoading.value = false
  }
}

const handleHistorySearch = async () => {
  historyError.value = ''
  historyLoading.value = true
  historySearched.value = true
  try {
    if (apiService.token === 'test-token-local') {
      await new Promise(r => setTimeout(r, 400))
      historyItems.value = MOCK_HISTORY
    } else {
      const filters = {}
      const f = historyFilters.value
      if (f.minCarbon !== '') filters.minCarbon = f.minCarbon
      if (f.maxCarbon !== '') filters.maxCarbon = f.maxCarbon
      if (f.aiScore !== '') filters.aiScore = f.aiScore
      if (f.startDate !== '') filters.startDate = f.startDate
      if (f.endDate !== '') filters.endDate = f.endDate
      historyItems.value = await apiService.getEvaluationHistory(filters)
    }
  } catch (e) {
    historyError.value = e.message || 'Erreur lors de la recherche'
  } finally {
    historyLoading.value = false
  }
}

const handleLogout = () => {
  apiService.logout()
  router.push('/login')
}

const resetForm = () => {
  formData.value = {
    modelName: '',
    provider: '',
    inputTokens: '',
    outputTokens: '',
    hoursSavedReports: '',
    hoursSavedImages: '',
    hoursSavedPresentations: '',
    dataSensitivity: 3,
    legalRisk: 3
  }
  showResults.value = false
  results.value = null
  formError.value = ''
}

const formatDate = (dateStr) => {
  if (!dateStr) return ''
  return new Date(dateStr).toLocaleDateString('fr-FR', { day: '2-digit', month: '2-digit', year: 'numeric' })
}

const fmt = (n, d = 6) => (n == null ? '-' : Number(n).toFixed(d))
</script>

<template>
  <div class="app-wrapper">
    <header class="header">
      <div class="header-content">
        <h1>EcoIA Évaluateur</h1>
        <button class="logout-btn" @click="handleLogout">Déconnexion</button>
      </div>
    </header>

    <div class="page-body">
      <!-- Left collapsible sidebar -->
      <div class="sidebar-wrapper" :class="{ open: sidebarOpen }">
        <div class="sidebar-inner">
          <button
            class="toggle-btn"
            @click="sidebarOpen = !sidebarOpen"
            :title="sidebarOpen ? 'Fermer' : 'Ouvrir les outils'"
          >
            {{ sidebarOpen ? '◀' : '▶' }}
          </button>

          <div v-show="sidebarOpen" class="sidebar-content">
            <!-- AI Score Panel -->
            <div class="sb-panel">
              <h3 class="panel-title">Notation IA</h3>
              <form @submit.prevent="handleScoreSubmit" class="sb-form">
                <div class="sb-field">
                  <label>ID Évaluation</label>
                  <input v-model="scoreForm.evaluationId" type="number" placeholder="Ex: 7">
                </div>
                <div class="sb-field">
                  <label>Note</label>
                  <input v-model="scoreForm.aiScore" type="text" placeholder="Ok, Utile, Inutile...">
                </div>
                <p v-if="scoreError" class="sb-error">{{ scoreError }}</p>
                <div v-if="scoreResult" class="sb-result">
                  <span class="mini-badge" :class="scoreResult.isApproved ? 'approved' : 'rejected'">
                    {{ scoreResult.isApproved ? 'APPROUVÉ' : 'REJETÉ' }}
                  </span>
                  <p class="sb-result-msg">{{ scoreResult.message }}</p>
                </div>
                <button type="submit" class="sb-btn" :disabled="scoreLoading">
                  {{ scoreLoading ? '...' : 'Envoyer la note' }}
                </button>
              </form>
            </div>

            <div class="panel-sep"></div>

            <!-- History Panel -->
            <div class="sb-panel">
              <h3 class="panel-title">Historique</h3>
              <form @submit.prevent="handleHistorySearch" class="sb-form">
                <div class="sb-field">
                  <label>Carbone min (kg)</label>
                  <input v-model="historyFilters.minCarbon" type="number" step="any" placeholder="Ex: 0.00000001">
                </div>
                <div class="sb-field">
                  <label>Carbone max (kg)</label>
                  <input v-model="historyFilters.maxCarbon" type="number" step="any" placeholder="Ex: 50">
                </div>
                <div class="sb-field">
                  <label>Note IA</label>
                  <input v-model="historyFilters.aiScore" type="text" placeholder="Ok, Utile...">
                </div>
                <div class="sb-field">
                  <label>Date début</label>
                  <input v-model="historyFilters.startDate" type="date">
                </div>
                <div class="sb-field">
                  <label>Date fin</label>
                  <input v-model="historyFilters.endDate" type="date">
                </div>
                <p v-if="historyError" class="sb-error">{{ historyError }}</p>
                <button type="submit" class="sb-btn" :disabled="historyLoading">
                  {{ historyLoading ? 'Recherche...' : 'Rechercher' }}
                </button>
              </form>

              <div v-if="historySearched && !historyLoading" class="history-list">
                <p v-if="historyItems.length === 0" class="no-results">Aucun résultat trouvé</p>
                <div v-for="item in historyItems" :key="item.id" class="history-item">
                  <div class="hi-top">
                    <span class="hi-model">{{ item.modelName }}</span>
                    <span class="hi-status" :class="item.isApproved ? 'approved' : 'rejected'">
                      {{ item.isApproved ? '✓' : '✗' }}
                    </span>
                  </div>
                  <div class="hi-meta">
                    <span>#{{ item.id }}</span>
                    <span v-if="item.aiScore" class="hi-score">{{ item.aiScore }}</span>
                  </div>
                  <div class="hi-values">
                    <span>🌱 {{ fmt(item.carbonFootprint, 4) }} kg</span>
                    <span>{{ formatDate(item.createdAt) }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Main content -->
      <div class="main-area">
        <!-- Evaluation Form -->
        <div class="form-card">
          <h2 class="form-title">Évaluer un modèle IA</h2>
          <p class="form-subtitle">Les champs marqués * sont obligatoires pour lancer l'analyse</p>

          <form @submit.prevent="handleSubmit" class="eval-form">
            <div class="form-row two-cols">
              <div class="form-group">
                <label>Modèle *</label>
                <input v-model="formData.modelName" type="text" placeholder="Ex: DeepSeek V3.1">
              </div>
              <div class="form-group">
                <label>Fournisseur *</label>
                <input v-model="formData.provider" type="text" placeholder="Ex: Référence, OpenAI">
              </div>
            </div>

            <div class="form-row two-cols">
              <div class="form-group">
                <label>Tokens d'entrée *</label>
                <input v-model="formData.inputTokens" type="number" min="0" placeholder="Ex: 5000">
              </div>
              <div class="form-group">
                <label>Tokens de sortie *</label>
                <input v-model="formData.outputTokens" type="number" min="0" placeholder="Ex: 15000">
              </div>
            </div>

            <div class="section-sep">Heures économisées par type de tâche</div>
            <div class="form-row three-cols">
              <div class="form-group">
                <label>Rapports</label>
                <input v-model="formData.hoursSavedReports" type="number" min="0" step="0.5" placeholder="Ex: 4.5">
              </div>
              <div class="form-group">
                <label>Images</label>
                <input v-model="formData.hoursSavedImages" type="number" min="0" step="0.5" placeholder="Ex: 0">
              </div>
              <div class="form-group">
                <label>Présentations</label>
                <input v-model="formData.hoursSavedPresentations" type="number" min="0" step="0.5" placeholder="Ex: 2">
              </div>
            </div>

            <div class="form-row two-cols">
              <div class="form-group">
                <label>Sensibilité des données — <strong>{{ formData.dataSensitivity }}/5</strong></label>
                <input v-model="formData.dataSensitivity" type="range" min="1" max="5" step="1" class="range-input">
                <div class="range-labels"><span>1 - Publique</span><span>5 - Très sensible</span></div>
              </div>
              <div class="form-group">
                <label>Risque légal — <strong>{{ formData.legalRisk }}/5</strong></label>
                <input v-model="formData.legalRisk" type="range" min="1" max="5" step="1" class="range-input">
                <div class="range-labels"><span>1 - Faible</span><span>5 - Très élevé</span></div>
              </div>
            </div>

            <p v-if="formError" class="form-error">{{ formError }}</p>

            <div class="form-actions">
              <button type="submit" class="submit-btn" :disabled="loading">
                {{ loading ? 'Calcul en cours...' : "Calculer l'impact" }}
              </button>
              <button type="button" class="reset-btn" @click="resetForm">Réinitialiser</button>
            </div>
          </form>
        </div>

        <!-- Results Section -->
        <div v-if="showResults && results" class="results-section">
          <div class="result-header" :class="results.isApproved ? 'hdr-approved' : 'hdr-rejected'">
            <div class="result-top">
              <span class="status-badge" :class="results.isApproved ? 'approved' : 'rejected'">
                {{ results.isApproved ? 'APPROUVÉ' : 'REJETÉ' }}
              </span>
              <span class="eval-id">Évaluation #{{ results.evaluationId }}</span>
            </div>
            <p class="result-message">{{ results.message }}</p>
          </div>

          <div class="metrics-grid">
            <div class="metric-card">
              <div class="metric-icon">⚡</div>
              <div class="metric-value">{{ fmt(results.totalEnergyKwh, 6) }}</div>
              <div class="metric-label">kWh — Énergie</div>
            </div>
            <div class="metric-card">
              <div class="metric-icon">🌱</div>
              <div class="metric-value">{{ fmt(results.totalCarbonKg, 6) }}</div>
              <div class="metric-label">kg CO₂ — Carbone</div>
            </div>
            <div class="metric-card">
              <div class="metric-icon">💧</div>
              <div class="metric-value">{{ fmt(results.totalWaterLiters, 6) }}</div>
              <div class="metric-label">Litres — Eau</div>
            </div>
            <div class="metric-card">
              <div class="metric-icon">💰</div>
              <div class="metric-value">{{ fmt(results.totalCostUsd, 4) }}</div>
              <div class="metric-label">USD — Coût</div>
            </div>
            <div class="metric-card">
              <div class="metric-icon">⏱</div>
              <div class="metric-value">{{ results.totalHoursSaved }}</div>
              <div class="metric-label">Heures économisées</div>
            </div>
            <div class="metric-card">
              <div class="metric-icon">⚠</div>
              <div class="metric-value">{{ results.riskScore }}<span class="metric-unit">/5</span></div>
              <div class="metric-label">Score de risque</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* ── Layout ─────────────────────────────────────────── */
.app-wrapper {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background: #f0f2f5;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.header {
  background-color: darkblue;
  color: white;
  padding: 1.2rem 0;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-content {
  max-width: 1400px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 1.5rem;
}

.header h1 {
  margin: 0;
  font-size: 1.6rem;
}

.logout-btn {
  background: rgba(255, 255, 255, 0.15);
  color: white;
  border: 2px solid rgba(255, 255, 255, 0.6);
  padding: 0.45rem 1rem;
  border-radius: 5px;
  cursor: pointer;
  font-weight: 600;
  transition: background 0.2s;
}

.logout-btn:hover {
  background: rgba(255, 255, 255, 0.28);
}

.page-body {
  display: flex;
  flex: 1;
  min-height: 0;
}

/* ── Sidebar ─────────────────────────────────────────── */
.sidebar-wrapper {
  width: 48px;
  flex-shrink: 0;
  background: #ffffff;
  border-right: 2px solid #e4e8ec;
  box-shadow: 2px 0 12px rgba(0, 0, 0, 0.06);
  overflow: hidden;
  transition: width 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  z-index: 50;
}

.sidebar-wrapper.open {
  width: 300px;
}

.sidebar-inner {
  width: 300px;
  display: flex;
  flex-direction: column;
  height: 100%;
}

.toggle-btn {
  width: 48px;
  height: 48px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: none;
  border: none;
  border-bottom: 1px solid #e4e8ec;
  cursor: pointer;
  font-size: 1rem;
  color: darkblue;
  flex-shrink: 0;
}

.toggle-btn:hover {
  background: #f5f7fa;
}

.sidebar-content {
  flex: 1;
  overflow-y: auto;
  padding: 1rem;
}

.sb-panel {
  margin-bottom: 0.5rem;
}

.panel-title {
  font-size: 0.95rem;
  font-weight: 700;
  color: darkblue;
  margin: 0 0 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.panel-sep {
  height: 1px;
  background: #e4e8ec;
  margin: 1.25rem 0;
}

.sb-form {
  display: flex;
  flex-direction: column;
  gap: 0.6rem;
}

.sb-field {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.sb-field label {
  font-size: 0.78rem;
  font-weight: 500;
  color: #555;
}

.sb-field input {
  padding: 0.45rem 0.6rem;
  border: 1.5px solid #ddd;
  border-radius: 4px;
  font-size: 0.85rem;
  transition: border-color 0.2s;
}

.sb-field input:focus {
  outline: none;
  border-color: darkblue;
}

.sb-btn {
  margin-top: 0.25rem;
  padding: 0.55rem;
  background: darkblue;
  color: white;
  border: none;
  border-radius: 4px;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  transition: opacity 0.2s;
}

.sb-btn:hover:not(:disabled) {
  opacity: 0.85;
}

.sb-btn:disabled {
  opacity: 0.55;
  cursor: not-allowed;
}

.sb-error {
  font-size: 0.78rem;
  color: #e74c3c;
  background: #fdf0f0;
  padding: 0.35rem 0.5rem;
  border-radius: 4px;
  margin: 0;
}

.sb-result {
  background: #f0f8f4;
  border-radius: 4px;
  padding: 0.5rem;
}

.sb-result-msg {
  font-size: 0.78rem;
  color: #444;
  margin: 0.35rem 0 0;
  line-height: 1.4;
}

.mini-badge {
  font-size: 0.7rem;
  font-weight: 700;
  padding: 0.2rem 0.5rem;
  border-radius: 3px;
  letter-spacing: 0.5px;
}

.mini-badge.approved { background: #d4edda; color: #155724; }
.mini-badge.rejected { background: #f8d7da; color: #721c24; }

/* History list */
.history-list {
  margin-top: 0.75rem;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.no-results {
  font-size: 0.82rem;
  color: #888;
  text-align: center;
  padding: 0.5rem;
}

.history-item {
  background: #f8f9fa;
  border: 1px solid #e4e8ec;
  border-radius: 5px;
  padding: 0.5rem 0.6rem;
  font-size: 0.78rem;
}

.hi-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.2rem;
}

.hi-model {
  font-weight: 600;
  color: #333;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  max-width: 190px;
}

.hi-status {
  font-weight: 700;
  font-size: 0.85rem;
}

.hi-status.approved { color: #27ae60; }
.hi-status.rejected { color: #e74c3c; }

.hi-meta {
  display: flex;
  gap: 0.5rem;
  color: #666;
  margin-bottom: 0.15rem;
}

.hi-score {
  background: #e8f0fe;
  color: darkblue;
  padding: 0 0.35rem;
  border-radius: 3px;
  font-weight: 500;
}

.hi-values {
  display: flex;
  justify-content: space-between;
  color: #555;
}

/* ── Main Content ─────────────────────────────────────── */
.main-area {
  flex: 1;
  padding: 2rem 2rem 3rem;
  overflow-y: auto;
  max-width: 900px;
  margin: 0 auto;
  width: 100%;
  box-sizing: border-box;
}

.form-card {
  background: white;
  padding: 2rem;
  border-radius: 10px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
}

.form-title {
  text-align: center;
  color: #222;
  font-size: 1.5rem;
  margin: 0 0 0.4rem;
}

.form-subtitle {
  text-align: center;
  color: #666;
  font-size: 0.9rem;
  margin: 0 0 1.75rem;
}

.eval-form {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
}

.form-row {
  display: grid;
  gap: 1rem;
}

.two-cols { grid-template-columns: 1fr 1fr; }
.three-cols { grid-template-columns: 1fr 1fr 1fr; }

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.4rem;
}

.form-group label {
  font-size: 0.9rem;
  font-weight: 500;
  color: #333;
}

.form-group input:not(.range-input) {
  padding: 0.7rem;
  border: 2px solid #ddd;
  border-radius: 5px;
  font-size: 0.95rem;
  font-family: inherit;
  transition: border-color 0.2s;
}

.form-group input:not(.range-input):focus {
  outline: none;
  border-color: darkblue;
  box-shadow: 0 0 0 3px rgba(0, 0, 128, 0.1);
}

.range-input {
  width: 100%;
  height: 6px;
  border-radius: 3px;
  accent-color: darkblue;
  cursor: pointer;
  margin-top: 0.3rem;
}

.range-labels {
  display: flex;
  justify-content: space-between;
  font-size: 0.75rem;
  color: #888;
  margin-top: 0.2rem;
}

.section-sep {
  font-size: 0.85rem;
  font-weight: 600;
  color: #555;
  border-bottom: 1px solid #eee;
  padding-bottom: 0.4rem;
  margin-bottom: -0.25rem;
}

.form-error {
  color: #e74c3c;
  background: #fdf0f0;
  border: 1px solid #f5c6cb;
  padding: 0.6rem 0.8rem;
  border-radius: 5px;
  font-size: 0.9rem;
  margin: 0;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
  margin-top: 0.5rem;
}

.submit-btn {
  padding: 0.85rem 2.5rem;
  background: darkblue;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: opacity 0.2s;
}

.submit-btn:hover:not(:disabled) { opacity: 0.87; }
.submit-btn:disabled { opacity: 0.55; cursor: not-allowed; }

.reset-btn {
  padding: 0.85rem 2rem;
  background: #f0f0f0;
  color: #333;
  border: 2px solid #ddd;
  border-radius: 5px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
}

.reset-btn:hover { background: #e4e4e4; }

/* ── Results ─────────────────────────────────────────── */
.results-section {
  margin-top: 2rem;
  animation: slideUp 0.4s ease;
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(16px); }
  to   { opacity: 1; transform: translateY(0); }
}

.result-header {
  border-radius: 10px;
  padding: 1.25rem 1.5rem;
  margin-bottom: 1.25rem;
}

.hdr-approved {
  background: #d4edda;
  border: 1.5px solid #b8dacc;
}

.hdr-rejected {
  background: #f8d7da;
  border: 1.5px solid #f5c6cb;
}

.result-top {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 0.5rem;
}

.status-badge {
  font-size: 0.8rem;
  font-weight: 800;
  padding: 0.3rem 0.75rem;
  border-radius: 4px;
  letter-spacing: 0.8px;
}

.status-badge.approved { background: #27ae60; color: white; }
.status-badge.rejected { background: #e74c3c; color: white; }

.eval-id {
  font-size: 0.9rem;
  color: #555;
  font-weight: 500;
}

.result-message {
  margin: 0;
  color: #333;
  font-size: 0.95rem;
  line-height: 1.5;
}

.metrics-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
}

.metric-card {
  background: white;
  border-radius: 8px;
  padding: 1.25rem;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.07);
  text-align: center;
  transition: transform 0.2s, box-shadow 0.2s;
}

.metric-icon {
  font-size: 1.8rem;
  margin-bottom: 0.5rem;
}

.metric-value {
  font-size: 1.3rem;
  font-weight: 700;
  color: darkblue;
  margin-bottom: 0.25rem;
  word-break: break-all;
}

.metric-unit {
  font-size: 0.85rem;
  font-weight: 400;
  color: #666;
}

.metric-label {
  font-size: 0.78rem;
  color: #888;
  text-transform: uppercase;
  letter-spacing: 0.4px;
}

/* ── Responsive ──────────────────────────────────────── */
@media (max-width: 700px) {
  .two-cols, .three-cols { grid-template-columns: 1fr; }
  .metrics-grid { grid-template-columns: 1fr 1fr; }
  .main-area { padding: 1rem; }
  .form-card { padding: 1.25rem; }
  .form-actions { flex-direction: column; }
  .submit-btn, .reset-btn { width: 100%; }
}
</style>
