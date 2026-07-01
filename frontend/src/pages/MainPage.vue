<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import apiService from '../services/apiService'

const router = useRouter()

// Sidebar
const sidebarOpen = ref(false)

const useCases = [
  "rédaction business", "code du quotidien (requête SQL...etc)", 
  "assistant quotidien", "code dev", "analyse de document", 
  "rédaction rapport", "décisions logique", "code technique (debug, algorithme)", 
  "raisonnement dans un probleme complexe"
]

const experienceLevels = ["junior", "confirmé", "senior", "expert"]
const complexities = ["petit", "grand"]
const models = ["GPT", "Claude", "DeepSeek", "Llama"]
const providers = ["OpenAI", "Anthropic", "Google", "AWS", "Azure"]


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
  aiModel: 'GPT',
  provider: 'Référence',
  useCase: 'rédaction business',
  complexity: 'grand',
  workflowDescription: '',
  runFrequency: 100,
  employeeCount: 1,
  hoursPerRun: 1,
  experienceLevel: 'confirmé',
  aiSavingsFraction: 0.4,
  inputTokens: 5000,
  outputTokens: 15000,
  dataSensitivity: 'interne', // Adapté en string pour ton backend
  legalRisk: 'faible'
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


const activeTab = ref('user')
const displayedMetrics = computed(() => {
  const r = results.value
  if (!r) return null

  const MIX_FRANCE = 0.0801; // Pour calculer le CO2 des recommandations

  switch (activeTab.value) {
    case 'env':
      return {
        label: "Alternative Green",
        model: `${r.recommendedEnvModel} (${r.recommendedEnvComplexity})`,
        energy: r.recommendedEnvEnergyKwh,
        carbon: r.recommendedEnvEnergyKwh * MIX_FRANCE,
        water: r.recommendedEnvWaterLiters,
        cost: r.recommendedEnvCostUsd
      }
    case 'eco':
      return {
        label: "Alternative FinOps",
        model: `${r.recommendedEcoModel} (${r.recommendedEcoComplexity})`,
        energy: r.recommendedEcoEnergyKwh,
        carbon: r.recommendedEcoEnergyKwh * MIX_FRANCE,
        water: r.recommendedEcoWaterLiters,
        cost: r.recommendedEcoCostUsd
      }
    case 'quality':
      return {
        label: "Alternative Premium",
        model: `${r.recommendedQualityModel} (${r.recommendedQualityComplexity})`,
        energy: r.recommendedQualityEnergyKwh,
        carbon: r.recommendedQualityEnergyKwh * MIX_FRANCE,
        water: r.recommendedQualityWaterLiters,
        cost: r.recommendedQualityCostUsd
      }
    default: // 'user' (Votre Choix)
      return {
        label: "Votre Sélection",
        model: `${formData.value.aiModel} (${formData.value.complexity})`,
        energy: r.totalEnergyKwh,
        carbon: r.totalCarbonKg,
        water: r.totalWaterLiters,
        cost: r.totalCostUsd,
        // --- Métriques exclusives à cet onglet ---
        valueSaved: r.valueSavedEur,
        efficiencyRating: r.efficiencyRating,
        envRating: r.environmentalRating,
        ecoRating: r.economicRating,
        riskScore: r.riskRating
      }
  }
})




const handleSubmit = async () => {
  formError.value = ''
  const d = formData.value
  
  // Validation stricte
  if (!d.aiModel || !d.provider || !d.inputTokens || !d.outputTokens) {
    formError.value = 'Veuillez remplir les champs obligatoires (modèle, fournisseur, tokens)'
    return
  }
  
  loading.value = true
  
  try {
    // Appel à ton API C# via le service
    const data = await apiService.calculateEvaluation(d)
    
    console.log("Réponse de l'API :", data)
    
    // --- ADAPTATION FRONT-END ---
    // On traduit la réponse du C# pour le template HTML
    results.value = {
      ...data,
      
      // Le front attend "message", le back envoie "verdictReason"
      message: data.verdictReason || "Aucun commentaire généré.",
      
      // Le front attend "riskScore", le back envoie "riskRating"
      riskScore: data.riskRating || 'N/A',
      
      // Le front attend un booléen "isApproved". 
      // D'après ton C#, tout ce qui n'est pas "Déconseillé" est accepté (Recommandé ou À optimiser)
      isApproved: data.verdictLevel === 'Recommandé' || data.verdictLevel === 'À optimiser'
    }
    
    showResults.value = true
    
  } catch (e) {
    console.error("Erreur attrapée :", e)
    formError.value = e.message || 'Erreur lors du calcul'
  } finally {
    loading.value = false
  }
}



const handleScoreSubmit = async () => {
  scoreError.value = ''
  scoreResult.value = null
  
  // 1. Validation : On vérifie les champs de scoreForm, pas formData
  if (!scoreForm.value.evaluationId || !scoreForm.value.aiScore) {
    scoreError.value = "Veuillez remplir l'ID et la note"
    return
  }
  
  // 2. On utilise scoreLoading au lieu de loading (pour ne pas bloquer le formulaire principal)
  scoreLoading.value = true
  
  try {
    const data = await apiService.updateEvaluationScore(
      scoreForm.value.evaluationId, 
      scoreForm.value.aiScore
    )
    scoreResult.value = data
    
    // Si l'évaluation est affichée à l'écran, on met à jour son statut
    if (results.value && String(results.value.evaluationId) === String(scoreForm.value.evaluationId)) {
      results.value = { ...results.value, isApproved: data.isApproved }
    }
  } catch (e) {
    scoreError.value = e.message || 'Erreur lors de la notation'
  } finally {
    scoreLoading.value = false
  }
}

onMounted(() => {
  handleHistorySearch()
})
const handleHistorySearch = async () => {
  historyError.value = ''
  historyLoading.value = true
  historySearched.value = true
  
  try {
    const filters = {}
    const f = historyFilters.value
    // N'ajoute les filtres que s'ils sont remplis
    if (f.minCarbon !== '') filters.minCarbon = f.minCarbon
    if (f.maxCarbon !== '') filters.maxCarbon = f.maxCarbon
    if (f.aiScore !== '') filters.aiScore = f.aiScore
    if (f.startDate !== '') filters.startDate = f.startDate
    if (f.endDate !== '') filters.endDate = f.endDate
    
    // Ton apiService va envoyer le token automatiquement
    historyItems.value = await apiService.getEvaluationHistory(filters)
  } catch (e) {
    historyError.value = e.message || 'Erreur lors de la récupération de l\'historique'
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
    aiModel: '',
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
      

      <!-- Main content -->
      <div class="main-area">
        <!-- Evaluation Form -->
        <div class="form-card">
          <h2 class="form-title">Évaluer un modèle IA</h2>
          <p class="form-subtitle">Les champs marqués * sont obligatoires pour lancer l'analyse</p>

          <form @submit.prevent="handleSubmit" class="eval-form">
            
            <div class="form-row two-cols">
            <div class="form-group">
              <label>Modèle IA *</label>
              <select v-model="formData.aiModel" class="select-input">
                <option v-for="m in models" :key="m" :value="m">{{ m }}</option>
              </select>
            </div>
            <div class="form-group">
              <label>Fournisseur *</label>
              <select v-model="formData.provider" class="select-input">
                <option v-for="p in providers" :key="p" :value="p">{{ p }}</option>
              </select>
            </div>
          </div>

            <div class="form-row two-cols">
              <div class="form-group">
                <label>Tokens d'entrée *</label>
                <input v-model="formData.inputTokens" type="number" min="0">
              </div>
              <div class="form-group">
                <label>Tokens de sortie *</label>
                <input v-model="formData.outputTokens" type="number" min="0">
              </div>
            </div>

            <div class="section-sep">Contexte & Cas d'usage</div>

            <div class="form-row two-cols">
              <div class="form-group">
                <label>Cas d'usage *</label>
                <select v-model="formData.useCase" class="select-input">
                  <option v-for="uc in useCases" :key="uc" :value="uc">{{ uc }}</option>
                </select>
              </div>
              <div class="form-group">
                <label>Complexité *</label>
                <select v-model="formData.complexity" class="select-input">
                  <option v-for="c in complexities" :key="c" :value="c">{{ c }}</option>
                </select>
              </div>
            </div>
            
            <div class="form-group">
              <label>Description du workflow</label>
              <input v-model="formData.workflowDescription" type="text" placeholder="Ex: Résumer les réunions d'équipe...">
            </div>

            <div class="form-row three-cols">
              <div class="form-group">
                <label>Fréquence (runs/mois)</label>
                <input v-model="formData.runFrequency" type="number" min="1">
              </div>
              <div class="form-group">
                <label>Employés mobilisés</label>
                <input v-model="formData.employeeCount" type="number" min="1">
              </div>
              <div class="form-group">
                <label>Heures par run</label>
                <input v-model="formData.hoursPerRun" type="number" step="0.1" min="0.1">
              </div>
            </div>

            <div class="form-row two-cols">
              <div class="form-group">
                <label>Niveau d'expérience</label>
                <select v-model="formData.experienceLevel" class="select-input">
                  <option v-for="lvl in experienceLevels" :key="lvl" :value="lvl">{{ lvl }}</option>
                </select>
              </div>
              <div class="form-group">
                <label>Gain de temps IA (%)</label>
                <input v-model="formData.aiSavingsFraction" type="number" step="0.1" min="0.1" max="1" placeholder="Ex: 0.4">
              </div>
            </div>

            <div class="section-sep">Risques & Conformité</div>

            <div class="form-row two-cols">
              <div class="form-group">
                <label>Sensibilité des données</label>
                <select v-model="formData.dataSensitivity" class="select-input">
                  <option value="public">Publique</option>
                  <option value="interne">Interne</option>
                  <option value="confidentiel">Confidentiel</option>
                  <option value="réglementé">Réglementé</option>
                </select>
              </div>
              <div class="form-group">
                <label>Risque légal</label>
                <select v-model="formData.legalRisk" class="select-input">
                  <option value="faible">Faible</option>
                  <option value="modéré">Modéré</option>
                  <option value="élevé">Élevé</option>
                  <option value="critique">Critique</option>
                </select>
              </div>
            </div>

            <p v-if="formError" class="form-error">{{ formError }}</p>

            <div class="form-actions">
              <button type="submit" class="submit-btn" :disabled="loading">
                {{ loading ? 'Calcul en cours...' : "Calculer l'impact" }}
              </button>
              <button type="button" class="reset-btn" @click="resetForm" :disabled="loading">
                Réinitialiser
              </button>
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

          <div class="tabs-container">
            <button class="tab-btn" :class="{ active: activeTab === 'user' }" @click="activeTab = 'user'">🎯 Votre Choix</button>
            <button class="tab-btn" :class="{ active: activeTab === 'env' }" @click="activeTab = 'env'">🌱 + Écolo</button>
            <button class="tab-btn" :class="{ active: activeTab === 'eco' }" @click="activeTab = 'eco'">💰 + Économe</button>
            <button class="tab-btn" :class="{ active: activeTab === 'quality' }" @click="activeTab = 'quality'">⚡ + Performant</button>
          </div>

          <div class="metrics-grid" v-if="displayedMetrics">
            
            <div class="metric-card highlight-card">
              <div class="metric-icon">🤖</div>
              <div class="metric-value model-name">{{ displayedMetrics.model }}</div>
              <div class="metric-label">{{ displayedMetrics.label }}</div>
            </div>

            <div class="metric-card">
              <div class="metric-icon">⚡</div>
              <div class="metric-value">{{ fmt(displayedMetrics.energy, 6) }}</div>
              <div class="metric-label">kWh — Énergie</div>
            </div>
            
            <div class="metric-card">
              <div class="metric-icon">🌱</div>
              <div class="metric-value">{{ fmt(displayedMetrics.carbon, 6) }}</div>
              <div class="metric-label">kg CO₂ — Carbone</div>
            </div>
            
            <div class="metric-card">
              <div class="metric-icon">💧</div>
              <div class="metric-value">{{ fmt(displayedMetrics.water, 6) }}</div>
              <div class="metric-label">Litres — Eau</div>
            </div>
            
            <div class="metric-card">
              <div class="metric-icon">💸</div>
              <div class="metric-value">{{ fmt(displayedMetrics.cost, 4) }}</div>
              <div class="metric-label">USD — Coût API</div>
            </div>

            <div class="metric-card" v-if="displayedMetrics.valueSaved !== undefined">
              <div class="metric-icon">💶</div>
              <div class="metric-value">{{ fmt(displayedMetrics.valueSaved, 2) }}</div>
              <div class="metric-label">EUR — Valeur Créée (Gain RH)</div>
            </div>

            <div class="metric-card" v-if="displayedMetrics.efficiencyRating !== undefined">
              <div class="metric-icon">⏱️</div>
              <div class="metric-value">{{ displayedMetrics.efficiencyRating }}<span class="metric-unit">/5</span></div>
              <div class="metric-label">Score Efficacité</div>
            </div>

            <div class="metric-card" v-if="displayedMetrics.envRating !== undefined">
              <div class="metric-icon">🌍</div>
              <div class="metric-value">{{ displayedMetrics.envRating }}<span class="metric-unit">/5</span></div>
              <div class="metric-label">Score GreenOps</div>
            </div>

            <div class="metric-card" v-if="displayedMetrics.ecoRating !== undefined">
              <div class="metric-icon">📈</div>
              <div class="metric-value">{{ displayedMetrics.ecoRating }}<span class="metric-unit">/5</span></div>
              <div class="metric-label">Score FinOps</div>
            </div>

            <div class="metric-card" v-if="displayedMetrics.riskScore !== undefined">
              <div class="metric-icon">⚠</div>
              <div class="metric-value">{{ displayedMetrics.riskScore }}<span class="metric-unit">/5</span></div>
              <div class="metric-label">Score de Risque</div>
            </div>

          </div>
        </div>
        </div>
      </div>


      <div class="right-sidebar">
        <div class="rs-header">
          <h3 class="panel-title">Vos Évaluations</h3>
          <button @click="handleHistorySearch" class="refresh-btn" :disabled="historyLoading">
            ↻
          </button>
        </div>

        <p v-if="historyError" class="sb-error">{{ historyError }}</p>
        <p v-if="historyLoading" class="loading-text">Chargement de l'historique...</p>
        
        <div v-else-if="historySearched" class="history-list">
          <p v-if="historyItems.length === 0" class="no-results">Aucun résultat trouvé</p>
          <div v-for="item in historyItems" :key="item.evaluationId || item.id" class="history-item">
            <div class="hi-top">
              <span class="hi-model">{{ item.modelName || item.aiModel }}</span>
              <span class="hi-status" :class="item.isApproved ? 'approved' : 'rejected'">
                {{ item.isApproved ? '✓' : '✗' }}
              </span>
            </div>
            <div class="hi-meta">
              <span>#{{ item.evaluationId || item.id }}</span>
              <span v-if="item.aiScore" class="hi-score">{{ item.aiScore }}</span>
            </div>
            <div class="hi-values">
              <span>🌱 {{ fmt(item.totalCarbonKg || item.carbonFootprint, 4) }} kg</span>
              <span>{{ formatDate(item.createdAt) }}</span>
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

.select-input {
  padding: 0.7rem;
  border: 2px solid #ddd;
  border-radius: 5px;
  font-size: 0.95rem;
  font-family: inherit;
  transition: border-color 0.2s;
  background-color: white;
  width: 100%;
  box-sizing: border-box;
}

.select-input:focus {
  outline: none;
  border-color: darkblue;
  box-shadow: 0 0 0 3px rgba(0, 0, 128, 0.1);
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
  width: 100%;
  max-width: 1400px; /* Limite la largeur totale */
  margin: 0 auto;
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


/* ── Tabs Navigation ─────────────────────────────────────────── */
.tabs-container {
  display: flex;
  gap: 0.5rem;
  margin-bottom: 1.5rem;
  background: #f8f9fa;
  padding: 0.5rem;
  border-radius: 8px;
  box-shadow: inset 0 2px 4px rgba(0,0,0,0.02);
}

.tab-btn {
  flex: 1;
  padding: 0.75rem;
  border: none;
  background: transparent;
  border-radius: 6px;
  font-weight: 600;
  color: #666;
  cursor: pointer;
  transition: all 0.2s ease;
  font-size: 0.9rem;
}

.tab-btn:hover {
  background: #e9ecef;
}

.tab-btn.active {
  background: darkblue;
  color: white;
  box-shadow: 0 2px 8px rgba(0, 0, 139, 0.25);
}

.highlight-card {
  background: #f4f6fb;
  border: 2px solid darkblue;
}

.model-name {
  font-size: 1.1rem !important;
  color: darkblue;
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

/* ── Right Sidebar (Historique) ───────────────────────── */
.right-sidebar {
  width: 320px;
  flex-shrink: 0;
  background: #ffffff;
  border-left: 2px solid #e4e8ec;
  padding: 1.5rem;
  height: calc(100vh - 80px); /* Hauteur moins le header */
  position: sticky;
  top: 80px; /* Colle la barre sous le header */
}

.rs-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.2rem;
  border-bottom: 1px solid #e4e8ec;
  padding-bottom: 0.5rem;
}

.refresh-btn {
  background: none;
  border: none;
  font-size: 1.2rem;
  color: darkblue;
  cursor: pointer;
  transition: transform 0.2s;
}

.refresh-btn:hover {
  transform: rotate(45deg);
}

.loading-text {
  font-size: 0.85rem;
  color: #666;
  text-align: center;
  margin-top: 1rem;
}

@media (max-width: 1100px) {
  .right-sidebar {
    display: none; /* Masque l'historique sur les petits écrans */
  }
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


@media (max-width: 700px) {
  .tabs-container { flex-direction: column; }
}


</style>
