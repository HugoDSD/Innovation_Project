<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import apiService from '../services/apiService'

const router = useRouter()

// Sidebar
const sidebarOpen = ref(false)
const showHistory = ref(true)

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
        <h1>SobrIA</h1>

        <div class="header-actions">
          <button class="history-btn" @click="showHistory = !showHistory">
            {{ showHistory ? 'Masquer historique' : 'Historique' }}
          </button>
          <button class="doc-btn-header" @click="router.push('/documentation')">
                Documentation
          </button>
          <button class="logout-btn" @click="handleLogout">Déconnexion</button>
        </div>
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
                <input v-model="formData.inputTokens" type="number" min="0", placeholder="Ex: 5000 tokens">
              </div>
              <div class="form-group">
                <label>Tokens de sortie *</label>
                <input v-model="formData.outputTokens" type="number" min="0", placeholder="Ex: 15000 tokens">
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
            <div class="result-top result-top-with-action">

              <span class="status-badge" :class="results.isApproved ? 'approved' : 'rejected'">
                {{ results.isApproved ? 'APPROUVÉ' : 'REJETÉ' }}
              </span>

              <button class="doc-btn" @click="router.push('/documentation')">
                Documentation
              </button>

            </div>
            <p class="result-message">{{ results.message }}</p>
          </div>

          <div class="tabs-container">
            <button
              class="tab-btn"
              :class="{ active: activeTab === 'user' }"
              @click="activeTab = 'user'"
            >
              Votre choix
            </button>

            <button
              class="tab-btn"
              :class="{ active: activeTab === 'env' }"
              @click="activeTab = 'env'"
            >
              Écologie
            </button>

            <button
              class="tab-btn"
              :class="{ active: activeTab === 'eco' }"
              @click="activeTab = 'eco'"
            >
              Économie
            </button>

            <button
              class="tab-btn"
              :class="{ active: activeTab === 'quality' }"
              @click="activeTab = 'quality'"
            >
              Performance
            </button>
          </div>
          <div class="metrics-wrapper" v-if="displayedMetrics">

            <!-- HERO MODEL -->
            <div class="metrics-hero">
              <div class="hero-label">Modèle utilisé</div>
              <div class="hero-value">{{ displayedMetrics.model }}</div>
            </div>

            <!-- ENV IMPACT -->
            <div class="metrics-block">

              <div class="metric-card big">
                <div class="label">Énergie</div>
                <div class="value">{{ fmt(displayedMetrics.energy, 6) }} kWh</div>
              </div>

              <div class="metric-card big">
                <div class="label">Carbone</div>
                <div class="value">{{ fmt(displayedMetrics.carbon, 6) }} kg CO₂</div>
              </div>

              <div class="metric-card big">
                <div class="label">Eau</div>
                <div class="value">{{ fmt(displayedMetrics.water, 6) }} L</div>
              </div>

            </div>

            <!-- COST -->
            <div class="metrics-cost">
              <div class="metric-card cost">
                <div class="label">Coût API</div>
                <div class="value">{{ fmt(displayedMetrics.cost, 4) }} USD</div>
              </div>
            </div>

            <!-- SCORES -->
            <div class="metrics-scores">

              <div class="metric-card small" v-if="displayedMetrics.valueSaved !== undefined">
                <div class="label">Valeur créée</div>
                <div class="value">{{ fmt(displayedMetrics.valueSaved, 2) }} €</div>
              </div>

              <div class="metric-card small" v-if="displayedMetrics.efficiencyRating !== undefined">
                <div class="label">Efficacité</div>
                <div class="value">{{ displayedMetrics.efficiencyRating }}/5</div>
              </div>

              <div class="metric-card small" v-if="displayedMetrics.envRating !== undefined">
                <div class="label">GreenOps</div>
                <div class="value">{{ displayedMetrics.envRating }}/5</div>
              </div>

              <div class="metric-card small" v-if="displayedMetrics.ecoRating !== undefined">
                <div class="label">FinOps</div>
                <div class="value">{{ displayedMetrics.ecoRating }}/5</div>
              </div>

              <div class="metric-card small" v-if="displayedMetrics.riskScore !== undefined">
                <div class="label">Risque</div>
                <div class="value">{{ displayedMetrics.riskScore }}/5</div>
              </div>

            </div>

          </div>
        </div>
        </div>
      </div>


      <div class="right-sidebar" v-if="showHistory">
        <div class="rs-header">
          <h3 class="panel-title">Historique des évaluations</h3>
          <button @click="handleHistorySearch" class="refresh-btn" :disabled="historyLoading">
            ↻
          </button>
        </div>

        <p v-if="historyError" class="sb-error">{{ historyError }}</p>
        <p v-if="historyLoading" class="loading-text">
          Chargement de l'historique...
        </p>

        <div v-else-if="historySearched" class="history-list">

          <p v-if="historyItems.length === 0" class="no-results">
            Aucun résultat trouvé
          </p>

          <div
            v-for="item in historyItems"
            :key="item.evaluationId || item.id"
            class="history-item"
          >

            <!-- HEADER -->
            <div class="hi-top">
              <span class="hi-model">
                {{ item.modelName || item.aiModel }}
              </span>

              <span class="hi-status" :class="item.isApproved ? 'approved' : 'rejected'">
                {{ item.isApproved ? 'APPROUVÉ' : 'REJETÉ' }}
              </span>
            </div>

            <!-- META -->
            <div class="hi-meta">
              <span>ID: {{ item.evaluationId || item.id }}</span>

              <span v-if="item.aiScore">
                Score IA: {{ item.aiScore }}
              </span>

              <span v-if="item.riskScore">
                Risque: {{ item.riskScore }}/5
              </span>
            </div>

            <!-- IMPACT -->
            <div class="hi-values">
              <span>
                CO₂: {{ fmt(item.totalCarbonKg || item.carbonFootprint, 4) }} kg
              </span>

              <span v-if="item.energyKwh">
                Énergie: {{ fmt(item.energyKwh, 4) }} kWh
              </span>
            </div>

            <div class="hi-values">
              <span v-if="item.waterFootprintLiters">
                Eau: {{ fmt(item.waterFootprintLiters, 4) }} L
              </span>

              <span v-if="item.costUsd">
                Coût: {{ fmt(item.costUsd, 4) }} $
              </span>
            </div>

            <div class="hi-values">
              <span v-if="item.hoursSaved">
                Temps gagné: {{ item.hoursSaved }} h
              </span>

              <span>
                {{ formatDate(item.createdAt) }}
              </span>
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
  background:  #194a3e;
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
  background: #407a69;
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

h1 {
  text-align: center;
  color: #ffffff;
  margin-bottom: 0.8rem;
  font-size: 2.3rem;
  font-weight: 700;
}

.logout-btn {
  padding-top: 1rem;
  padding-bottom: 1rem;
  padding-right: 1.5rem;
  padding-left: 1.5rem;
  border: none;
  border-radius: 12px;
  background: linear-gradient(135deg, #3fbf8f, #2f9d74);
  color: white;
  font-size: 1.1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.25s ease;
}

.logout-btn:hover {
  transform: translateY(-2px);
  opacity: 0.95;
}

.logout-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.page-body {
  display: flex;
  flex: 1;
  width: 100%;
  max-width: 1400px; /* Limite la largeur totale */
  margin: 0 auto;
}

input[type="number"]::-webkit-outer-spin-button,
input[type="number"]::-webkit-inner-spin-button {
  -webkit-appearance: none;
  margin: 0;
}

input[type="number"] {
  -moz-appearance: textfield;
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
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-radius: 12px;
  padding: 4px;
  gap: 4px;
  backdrop-filter: blur(10px);
  margin-bottom: 1.5rem;
  padding-top: 0.3rem;
  padding-bottom: 0.3rem;
}

/* bouton base */
.tab-btn {
  flex: 1;
  padding: 0.7rem 1rem;
  border: none;
  background: transparent;
  color: rgba(255, 255, 255, 0.75);
  font-size: 0.9rem;
  font-weight: 500;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.2s ease;
}

/* hover soft */
.tab-btn:hover {
  background: rgba(255, 255, 255, 0.08);
  color: white;
}

/* actif */
.tab-btn.active {
  background: linear-gradient(135deg, #3fbf8f, #2f9d74);
  color: white;
  box-shadow: 0 6px 18px rgba(47, 157, 116, 0.25);
}

/* focus clean (accessibilité) */
.tab-btn:focus {
  outline: none;
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

  /* cohérence login */
  background: transparent;
}

.form-card {
  max-width: 1000px;
  margin: 0 auto;

  padding: 2.5rem;

  background: #407a69;
  backdrop-filter: blur(18px);

  border-radius: 18px;

  border: 1px solid rgba(88, 199, 154, 0.25);
  transition: transform 0.25s ease, box-shadow 0.25s ease;
  color: white;
}

.form-title {
  text-align: center;
  color: #ffffff;
  font-size: 1.8rem;
  font-weight: 700;
  margin: 0 0 0.5rem;
}

.form-subtitle {
  text-align: center;
  color: rgba(255, 255, 255, 0.85);
  margin-bottom: 2.5rem;
  font-size: 1.05rem;
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
  margin-bottom: 1.6rem;
}

.form-group label {
  font-size: 0.9rem;
  font-weight: 500;
  display: block;
  margin-bottom: 0rem;
  color: white;
}

.form-group input,
.select-input {
  width: 100%;
  padding: 0.9rem 1rem;

  border-radius: 12px;

  border: 1px solid rgba(255, 255, 255, 0.18);

  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(10px);

  color: white;
  font-size: 0.95rem;

  transition: all 0.25s ease;
}

.form-group input:focus,
.select-input:focus {
  outline: none;

  border-color: rgba(88, 199, 154, 0.8);

  box-shadow:
    0 0 0 4px rgba(88, 199, 154, 0.15),
    0 8px 20px rgba(0, 0, 0, 0.15);

  transform: translateY(-1px);
}

.select-input:focus {
  background: #4f8475;
  border-radius: 15px;
}

.form-group input::placeholder {
  color: rgba(255, 255, 255, 0.45);
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
  padding: 1rem 2rem;
  border: none;
  border-radius: 12px;

  background: linear-gradient(135deg, #3fbf8f, #2f9d74);
  color: white;

  font-size: 1.1rem;
  font-weight: 600;

  cursor: pointer;

  transition: all 0.25s ease;
  box-shadow: 0 10px 25px rgba(47, 157, 116, 0.25);
}

.submit-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  opacity: 0.95;
  box-shadow: 0 14px 35px rgba(47, 157, 116, 0.35);
}

.submit-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  transform: none;
  box-shadow: none;
}

.reset-btn {
  padding: 1rem 2rem;

  border-radius: 12px;
  border: 1px solid rgba(255, 255, 255, 0.25);

  background: rgba(255, 255, 255, 0.08);
  backdrop-filter: blur(10px);

  color: rgba(255, 255, 255, 0.85);

  font-size: 1.05rem;
  font-weight: 500;

  cursor: pointer;

  transition: all 0.25s ease;
}

.reset-btn:hover {
  transform: translateY(-2px);
  background: rgba(255, 255, 255, 0.15);
  border-color: rgba(88, 199, 154, 0.4);
  color: #ffffff;
}

.reset-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  transform: none;
}


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

.status-badge.approved {
  background: linear-gradient(135deg, #3fbf8f, #2f9d74);
  color: white;
}

.status-badge.rejected {
  background: linear-gradient(135deg, #e85d5d, #c94242);
  color: white;
}

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

.metrics-wrapper {
  max-width: 750px;
  margin: 2rem auto;
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  align-items: center;
}

/* HERO MODEL (FORT IMPACT) */
.metrics-hero {
  width: 100%;
  background: linear-gradient(135deg, #3fbf8f, #2f9d74);
  border-radius: 18px;
  padding: 2rem;
  text-align: center;
  color: white;
  box-shadow: 0 15px 40px rgba(0,0,0,0.25);
}

.hero-label {
  font-size: 0.85rem;
  opacity: 0.85;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.hero-value {
  font-size: 1.6rem;
  font-weight: 800;
  margin-top: 0.5rem;
}

/* ENV BLOCK */
.metrics-block {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 1rem;
  width: 100%;
}

/* COST centered but strong */
.metrics-cost {
  width: 60%;
}

.metric-card {
  background: rgba(255, 255, 255, 0.10);
  border: 1px solid rgba(255, 255, 255, 0.18);
  border-radius: 14px;
  padding: 1.5rem;
  text-align: center;
  backdrop-filter: blur(12px);
  color: white;
}

/* BIG cards (env impact) */
.metric-card.big .value {
  font-size: 1.25rem;
  font-weight: 700;
}

/* COST card = special highlight */
.metric-card.cost {
  border: none;
  color: white;
}

/* SCORES */
.metrics-scores {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 0.8rem;
  width: 100%;
}

/* SMALL cards */
.metric-card.small {
  padding: 1rem;
  font-size: 0.9rem;
  opacity: 0.95;
}

/* labels */
.label {
  font-size: 0.7rem;
  text-transform: uppercase;
  opacity: 0.7;
  letter-spacing: 0.8px;
}

/* values */
.value {
  font-size: 1.05rem;
  font-weight: 700;
  margin-top: 0.3rem;
}

/* RESPONSIVE */
@media (max-width: 900px) {
  .metrics-block {
    grid-template-columns: 1fr;
  }

  .metrics-scores {
    grid-template-columns: 1fr 1fr;
  }

  .metrics-cost {
    width: 100%;
  }
}

/* ── Right Sidebar (Historique) ───────────────────────── */
.right-sidebar {
  position: fixed;
  top: 80px;
  right: 0;
  width: 320px;
  height: calc(100vh - 80px);

  background: #407a69;

  padding: 1.5rem;

  z-index: 9999;
  overflow-y: auto;
  color: white;
}

.rs-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.2rem;
  padding-bottom: 0.8rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.15);
}

.panel-title {
  font-size: 1rem;
  font-weight: 700;
  color: white;
  margin: 0;
  letter-spacing: 0.5px;
  text-transform: uppercase;
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

.result-top-with-action {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 1rem;
}

.doc-btn {
  padding: 0.5rem 0.9rem;
  border-radius: 10px;
  border: 1px solid rgba(255,255,255,0.25);
  background: linear-gradient(135deg, #3fbf8f, #2f9d74);
  color: white;
  font-size: 0.9rem;
  cursor: pointer;
  transition: all 0.2s ease;
  backdrop-filter: blur(10px);
}

.doc-btn:hover {
  transform: translateY(-1px);
}

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

.header-actions {
  display: flex;
  align-items: center;
  gap: 0.8rem;
}

.history-btn {
  padding: 0.6rem 1rem;
  border-radius: 10px;
  border: 1px solid rgba(255,255,255,0.25);
  background: rgba(255,255,255,0.08);
  color: white;
  cursor: pointer;
  transition: all 0.2s ease;
  backdrop-filter: blur(10px);
}

.history-btn:hover {
  background: rgba(255,255,255,0.15);
  transform: translateY(-1px);
}

.doc-btn-header {
  padding: 0.6rem 1rem;
  border-radius: 10px;
  border: 1px solid rgba(255,255,255,0.25);
  background: rgba(255,255,255,0.08);
  color: white;
  cursor: pointer;
  transition: all 0.2s ease;
  backdrop-filter: blur(10px);
}

.doc-btn-header:hover {
  background: rgba(255,255,255,0.15);
  transform: translateY(-1px);
}


.history-item {
  background: rgba(255, 255, 255, 0.08);
  border: 1px solid rgba(255, 255, 255, 0.15);
  border-radius: 14px;

  padding: 0.8rem 0.9rem;
  font-size: 0.8rem;

  color: white;

  backdrop-filter: blur(10px);

  cursor: pointer;
  transition: all 0.25s ease;
}

.history-item:hover {
  background: rgba(255, 255, 255, 0.14);
  transform: translateY(-2px);
  border-color: rgba(88, 199, 154, 0.4);
}

.hi-model {
  font-weight: 600;
  color: white;
  max-width: 180px;
}

.hi-meta,
.hi-values {
  color: rgba(255, 255, 255, 0.75);
}

.hi-status.approved {
  color: #3fbf8f;
}

.hi-status.rejected {
  color: #ff6b6b;
}

</style>