# MVP Implementation Plan

> **For agentic workers:** Use `superpowers:subagent-driven-development` or `superpowers:executing-plans` to execute this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build the frontend UI to match the backend API contract exactly, then wire it up.

**Architecture:** Two phases. Phase 1 builds the entire frontend with mock data shaped exactly like real backend responses — so Phase 2 (connection) requires zero component changes, only service wiring. Phase 3 cleans up backend comments.

**Tech stack:** Vue 3 · Vite · Vue Router 5 · .NET 10 · ASP.NET Core · PostgreSQL · JWT Bearer

## Global Constraints

- Code in English — variable names, function names, comments, class names
- UI in French — all text displayed to the user
- Backend runs at `http://localhost:5051`
- Frontend runs at `http://localhost:5173`
- JWT stored in `localStorage` under the key `token`
- Mock data must match the exact shape of real backend responses (same field names, same types)

---

# Phase 1 — Frontend (mock data)

---

## Task 1 — 2-step evaluation wizard

Replace the current single-page form with a 2-step wizard whose fields map exactly to `EvaluationRequestDto`.

**Backend contract (`EvaluationRequestDto`):**

```csharp
string ModelName     // "GPT OSS 20B" | "GPT OSS 120B" | "DeepSeek V3.1" | "DeepSeek R1"
string Provider      // "Microsoft" | "Amazon" | "Référence"
long   InputTokens
long   OutputTokens
double HoursSavedReports
double HoursSavedImages
double HoursSavedPresentations
int    DataSensitivity   // 1–5
int    LegalRisk         // 1–5
```

`projectName` and `projectDescription` are collected in step 1 for display only — not sent to the backend.

**Files:**
- Modify: `frontend/src/pages/MainPage.vue`

- [ ] **Replace the script block** with wizard state and a mock submit that returns data shaped like `EvaluationResultDto`:

```javascript
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import ResultsSection from '../components/ResultsSection.vue'

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

// Temporary mock — replaced in Phase 2 by a real API call
const mockSubmit = () => {
  const hoursSaved = step1.value.hoursSavedReports + step1.value.hoursSavedImages + step1.value.hoursSavedPresentations
  const riskScore = (step1.value.dataSensitivity + step1.value.legalRisk) / 2
  const approved = hoursSaved > 1.0 && riskScore < 4.0

  return {
    isApproved: approved,
    evaluationId: 1,
    message: approved
      ? 'APPROUVÉ : Le gain social compense l\'impact environnemental et les risques sont maîtrisés.'
      : 'REJETÉ : Les bénéfices ne compensent pas les risques ou l\'impact.',
    totalEnergyKwh: (step2.value.inputTokens + step2.value.outputTokens) * 3.708611e-7,
    totalCarbonKg: (step2.value.inputTokens + step2.value.outputTokens) * 3.708611e-7 * 0.0801,
    totalWaterLiters: (step2.value.inputTokens + step2.value.outputTokens) * 3.708611e-7 * 0.84,
    totalCostUsd: step2.value.inputTokens * 1.4e-7 + step2.value.outputTokens * 2.8e-7,
    totalHoursSaved: hoursSaved,
    riskScore
  }
}

const handleSubmit = async () => {
  if (!validateStep2()) return
  loading.value = true
  error.value = ''
  await new Promise(r => setTimeout(r, 600)) // Simulate network delay
  results.value = mockSubmit()
  loading.value = false
  setTimeout(() => {
    document.querySelector('.results-section')?.scrollIntoView({ behavior: 'smooth' })
  }, 100)
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
```

- [ ] **Replace the template** with the 2-step wizard:

```html
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
```

- [ ] **Update styles** — add step indicator and nav button, keep existing styles:

```css
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

.step-indicator .divider { color: #ccc; }

.section-title {
  font-size: 1rem;
  color: #333;
  margin: 0.5rem 0;
  font-weight: 600;
}
```

- [ ] **Test** — fill step 1, click Suivant, fill step 2, click Analyser. Confirm the mock results appear and match the expected shape. Confirm Back preserves step 1 data. Confirm Reset clears both steps.

- [ ] **Commit**

```bash
git add frontend/src/pages/MainPage.vue
git commit -m "feat: 2-step evaluation wizard with mock data matching backend contract"
```

---

## Task 2 — Results components (correct data structure)

Update all result components to use the exact field names from `EvaluationResultDto`. Remove mock-specific fields. No API call yet.

**Backend contract (`EvaluationResultDto`):**

```csharp
bool   IsApproved
int    EvaluationId
string Message
double TotalEnergyKwh
double TotalCarbonKg
double TotalWaterLiters
double TotalCostUsd
double TotalHoursSaved
double RiskScore
```

**Files:**
- Modify: `frontend/src/components/ResultsSection.vue`
- Modify: `frontend/src/components/AILevelIndicator.vue`
- Modify: `frontend/src/components/EnvironmentalImpactCard.vue`
- Modify: `frontend/src/components/BusinessImpactCard.vue`
- Delete: `frontend/src/components/ImpactCard.vue` (defined but never used)

- [ ] **Rewrite `AILevelIndicator.vue`** — binary verdict, not 4-level scale:

```vue
<script setup>
defineProps({
  isApproved: { type: Boolean, required: true },
  message: { type: String, default: '' }
})
</script>

<template>
  <div class="verdict-container" :class="isApproved ? 'approved' : 'rejected'">
    <span class="verdict-icon">{{ isApproved ? '✓' : '✗' }}</span>
    <div>
      <p class="verdict-label">{{ isApproved ? 'Approuvé' : 'Rejeté' }}</p>
      <p class="verdict-message">{{ message }}</p>
    </div>
  </div>
</template>

<style scoped>
.verdict-container {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1.25rem 1.5rem;
  border-radius: 8px;
  margin-bottom: 2rem;
}

.approved { background-color: #d4edda; color: #155724; }
.rejected { background-color: #f8d7da; color: #721c24; }

.verdict-icon { font-size: 2rem; font-weight: bold; flex-shrink: 0; }
.verdict-label { margin: 0; font-size: 1.1rem; font-weight: 700; }
.verdict-message { margin: 0.25rem 0 0; font-size: 0.9rem; }
</style>
```

- [ ] **Rewrite `EnvironmentalImpactCard.vue`** — three real metrics, no stage breakdown:

```vue
<script setup>
defineProps({
  carbonKg:    { type: Number, required: true },
  waterLiters: { type: Number, required: true },
  energyKwh:   { type: Number, required: true }
})
</script>

<template>
  <div class="environmental-card">
    <h4 class="card-title">Impact Environnemental</h4>

    <div class="metric">
      <span class="metric-value">{{ carbonKg.toFixed(6) }}</span>
      <span class="metric-unit">kg CO₂</span>
      <p class="metric-label">Empreinte carbone</p>
    </div>
    <div class="metric">
      <span class="metric-value">{{ waterLiters.toFixed(4) }}</span>
      <span class="metric-unit">L</span>
      <p class="metric-label">Consommation d'eau</p>
    </div>
    <div class="metric">
      <span class="metric-value">{{ energyKwh.toFixed(6) }}</span>
      <span class="metric-unit">kWh</span>
      <p class="metric-label">Énergie consommée</p>
    </div>
  </div>
</template>

<style scoped>
.environmental-card {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  border-top: 4px solid #27ae60;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}
.card-title { margin: 0 0 1.25rem; color: #333; font-size: 1.1rem; font-weight: 600; }
.metric { background: #f8f9fa; border-left: 3px solid #27ae60; padding: 0.75rem 1rem; border-radius: 4px; margin-bottom: 0.75rem; }
.metric-value { font-size: 1.4rem; font-weight: bold; color: #27ae60; }
.metric-unit { font-size: 0.9rem; color: #27ae60; margin-left: 0.25rem; }
.metric-label { margin: 0.25rem 0 0; color: #666; font-size: 0.85rem; }
</style>
```

- [ ] **Rewrite `BusinessImpactCard.vue`** — cost, hours saved, risk score:

```vue
<script setup>
defineProps({
  costUsd:    { type: Number, required: true },
  hoursSaved: { type: Number, required: true },
  riskScore:  { type: Number, required: true }
})
</script>

<template>
  <div class="business-card">
    <h4 class="card-title">Impact Métier</h4>

    <div class="metric">
      <span class="metric-value">${{ costUsd.toFixed(4) }}</span>
      <p class="metric-label">Coût estimé</p>
    </div>
    <div class="metric">
      <span class="metric-value">{{ hoursSaved.toFixed(1) }} h</span>
      <p class="metric-label">Temps économisé</p>
    </div>
    <div class="metric" :class="riskScore >= 4 ? 'high-risk' : 'low-risk'">
      <span class="metric-value">{{ riskScore.toFixed(1) }} / 5</span>
      <p class="metric-label">Score de risque</p>
    </div>
  </div>
</template>

<style scoped>
.business-card {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  border-top: 4px solid #267bf1;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
}
.card-title { margin: 0 0 1.25rem; color: #333; font-size: 1.1rem; font-weight: 600; }
.metric { background: #f8f9fa; border-left: 3px solid #267bf1; padding: 0.75rem 1rem; border-radius: 4px; margin-bottom: 0.75rem; }
.metric.high-risk { border-left-color: #e74c3c; }
.metric.low-risk  { border-left-color: #27ae60; }
.metric-value { font-size: 1.4rem; font-weight: bold; color: #267bf1; }
.metric-label { margin: 0.25rem 0 0; color: #666; font-size: 0.85rem; }
</style>
```

- [ ] **Update `ResultsSection.vue`** — use new prop names, pass correct fields:

```vue
<script setup>
import AILevelIndicator from './AILevelIndicator.vue'
import EnvironmentalImpactCard from './EnvironmentalImpactCard.vue'
import BusinessImpactCard from './BusinessImpactCard.vue'

defineProps({
  results: { type: Object, default: null }
})
</script>

<template>
  <div v-if="results" class="results-section">
    <h2 class="results-title">Résultats de l'analyse</h2>

    <AILevelIndicator
      :is-approved="results.isApproved"
      :message="results.message"
    />

    <div class="impact-grid">
      <EnvironmentalImpactCard
        :carbon-kg="results.totalCarbonKg"
        :water-liters="results.totalWaterLiters"
        :energy-kwh="results.totalEnergyKwh"
      />
      <BusinessImpactCard
        :cost-usd="results.totalCostUsd"
        :hours-saved="results.totalHoursSaved"
        :risk-score="results.riskScore"
      />
    </div>
  </div>
</template>

<!-- Keep existing styles unchanged -->
```

- [ ] **Delete the unused component:**

```bash
rm frontend/src/components/ImpactCard.vue
```

- [ ] **Test** — run the wizard, confirm the results panel shows all three sections with correct values. Flip `isApproved` in the mock to check both verdict colours.

- [ ] **Commit**

```bash
git add frontend/src/components/
git commit -m "feat: update results components to match EvaluationResultDto contract"
```

---

## Task 3 — AI score rating (UI only)

Inline rating widget shown after results. No API call yet — just state and UI.

**Files:**
- Create: `frontend/src/components/AIScoreRating.vue`
- Modify: `frontend/src/components/ResultsSection.vue`

The four score values must match exactly what the backend stores in `AiScore`:
`'Utile'` · `'Moyen'` · `'Non utile'` · `'Mieux sans IA'`

- [ ] **Create `AIScoreRating.vue`**:

```vue
<script setup>
import { ref } from 'vue'

defineProps({
  evaluationId: { type: Number, required: true }
})

const SCORES = ['Utile', 'Moyen', 'Non utile', 'Mieux sans IA']
const selected = ref(null)
const submitted = ref(false)

// Replaced in Phase 2 by a real API call
const submitScore = () => {
  if (selected.value) submitted.value = true
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
        :disabled="!selected"
        @click="submitScore"
      >
        Confirmer
      </button>
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
</style>
```

- [ ] **Add `AIScoreRating` to `ResultsSection.vue`**:

```vue
<script setup>
import AILevelIndicator from './AILevelIndicator.vue'
import EnvironmentalImpactCard from './EnvironmentalImpactCard.vue'
import BusinessImpactCard from './BusinessImpactCard.vue'
import AIScoreRating from './AIScoreRating.vue'

defineProps({
  results: { type: Object, default: null }
})
</script>

<template>
  <div v-if="results" class="results-section">
    <h2 class="results-title">Résultats de l'analyse</h2>

    <AILevelIndicator
      :is-approved="results.isApproved"
      :message="results.message"
    />

    <div class="impact-grid">
      <EnvironmentalImpactCard
        :carbon-kg="results.totalCarbonKg"
        :water-liters="results.totalWaterLiters"
        :energy-kwh="results.totalEnergyKwh"
      />
      <BusinessImpactCard
        :cost-usd="results.totalCostUsd"
        :hours-saved="results.totalHoursSaved"
        :risk-score="results.riskScore"
      />
    </div>

    <AIScoreRating :evaluation-id="results.evaluationId" />
  </div>
</template>
```

- [ ] **Test** — after seeing results, click each score button, confirm selection highlights. Click Confirmer, confirm the confirmation message appears. Confirm the widget resets when the form is reset.

- [ ] **Commit**

```bash
git add frontend/src/components/AIScoreRating.vue frontend/src/components/ResultsSection.vue
git commit -m "feat: add AI score rating widget (UI only)"
```

---

## Task 4 — History page (UI only)

A dedicated page showing mock history entries shaped like `EvaluationHistoryDto`.

**Backend contract (`EvaluationHistoryDto`):**

```csharp
string   Id
string   ModelName
string   AiScore
double   CarbonFootprint
double   WaterFootprintLiters
double   EnergyKwh
double   CostUsd
double   HoursSaved
double   RiskScore
bool     IsApproved
DateTime CreatedAt
```

**Files:**
- Create: `frontend/src/pages/HistoryPage.vue`
- Modify: `frontend/src/router.js`

- [ ] **Add the `/history` route in `router.js`**:

```javascript
import HistoryPage from './pages/HistoryPage.vue'

// Add to routes array:
{
  path: '/history',
  name: 'History',
  component: HistoryPage,
  meta: { requiresAuth: true }
}
```

- [ ] **Create `HistoryPage.vue`** with mock data shaped exactly like `EvaluationHistoryDto`:

```vue
<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()

// Mock data — replaced in Phase 2 by getHistory() API call
const history = ref([
  {
    id: '1',
    modelName: 'DeepSeek V3.1',
    aiScore: 'Utile',
    carbonFootprint: 0.0000594,
    waterFootprintLiters: 0.000623,
    energyKwh: 0.000742,
    costUsd: 0.0049,
    hoursSaved: 6.5,
    riskScore: 2.5,
    isApproved: true,
    createdAt: new Date().toISOString()
  },
  {
    id: '2',
    modelName: 'GPT OSS 20B',
    aiScore: '',
    carbonFootprint: 0.0000021,
    waterFootprintLiters: 0.0000026,
    energyKwh: 0.0000261,
    costUsd: 0.0001,
    hoursSaved: 0.5,
    riskScore: 4.0,
    isApproved: false,
    createdAt: new Date(Date.now() - 86400000).toISOString()
  }
])

const loading = ref(false)
const error = ref('')

const formatDate = (iso) =>
  new Date(iso).toLocaleDateString('fr-FR', { day: '2-digit', month: '2-digit', year: 'numeric' })

const logout = () => {
  localStorage.removeItem('token')
  router.push('/login')
}
</script>

<template>
  <div class="history-container">
    <header class="header">
      <div class="header-content">
        <h1>Évaluateur d'Impact IA</h1>
        <div class="header-actions">
          <button class="nav-btn" @click="router.push('/app')">Nouvelle analyse</button>
          <button class="logout-btn" @click="logout">Déconnexion</button>
        </div>
      </div>
    </header>

    <div class="content">
      <h2 class="page-title">Historique des évaluations</h2>

      <p v-if="loading" class="status">Chargement...</p>
      <p v-else-if="error" class="error-message">{{ error }}</p>
      <p v-else-if="history.length === 0" class="status">Aucune évaluation pour le moment.</p>

      <div v-else class="history-list">
        <div
          v-for="item in history"
          :key="item.id"
          class="history-card"
          :class="item.isApproved ? 'approved' : 'rejected'"
        >
          <div class="card-header">
            <span class="model-name">{{ item.modelName }}</span>
            <span class="verdict-badge">{{ item.isApproved ? 'Approuvé' : 'Rejeté' }}</span>
            <span v-if="item.aiScore" class="ai-score-badge">{{ item.aiScore }}</span>
            <span class="date">{{ formatDate(item.createdAt) }}</span>
          </div>
          <div class="card-metrics">
            <span>{{ item.carbonFootprint.toFixed(6) }} kg CO₂</span>
            <span>{{ item.waterFootprintLiters.toFixed(4) }} L</span>
            <span>{{ item.energyKwh.toFixed(6) }} kWh</span>
            <span>${{ item.costUsd.toFixed(4) }}</span>
            <span>{{ item.hoursSaved.toFixed(1) }} h économisées</span>
            <span>Risque : {{ item.riskScore.toFixed(1) }}/5</span>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.history-container { min-height: 100vh; background-color: lightgrey; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; }
.header { background-color: darkblue; color: white; padding: 1.5rem 0; position: sticky; top: 0; z-index: 100; box-shadow: 0 4px 12px rgba(0,0,0,0.15); }
.header-content { max-width: 900px; margin: 0 auto; display: flex; justify-content: space-between; align-items: center; padding: 0 1.5rem; }
.header-content h1 { margin: 0; font-size: 1.8rem; }
.header-actions { display: flex; gap: 0.75rem; }
.nav-btn { background: transparent; color: white; border: 2px solid white; padding: 0.5rem 1rem; border-radius: 5px; cursor: pointer; font-weight: 600; }
.logout-btn { background: rgba(255,255,255,0.2); color: white; border: 2px solid white; padding: 0.5rem 1rem; border-radius: 5px; cursor: pointer; font-weight: 600; }
.content { max-width: 900px; margin: 2rem auto; padding: 0 1.5rem; }
.page-title { color: #333; font-size: 1.5rem; margin-bottom: 1.5rem; }
.status { color: #666; }
.error-message { color: #e74c3c; }
.history-list { display: flex; flex-direction: column; gap: 1rem; }
.history-card { background: white; border-radius: 8px; padding: 1.25rem; border-left: 5px solid #ccc; box-shadow: 0 2px 8px rgba(0,0,0,0.08); }
.history-card.approved { border-left-color: #27ae60; }
.history-card.rejected { border-left-color: #e74c3c; }
.card-header { display: flex; align-items: center; gap: 0.75rem; margin-bottom: 0.75rem; flex-wrap: wrap; }
.model-name { font-weight: 600; color: #333; }
.verdict-badge { font-size: 0.8rem; font-weight: 600; padding: 0.2rem 0.6rem; border-radius: 10px; }
.approved .verdict-badge { background: #d4edda; color: #155724; }
.rejected .verdict-badge { background: #f8d7da; color: #721c24; }
.ai-score-badge { background: #e8eaf6; color: #333; padding: 0.2rem 0.6rem; border-radius: 10px; font-size: 0.8rem; font-weight: 600; }
.date { color: #999; font-size: 0.85rem; margin-left: auto; }
.card-metrics { display: flex; flex-wrap: wrap; gap: 1rem; font-size: 0.85rem; color: #555; }
</style>
```

- [ ] **Test** — navigate to `/history` from the main page, confirm two mock entries display. Confirm one shows an AI score badge and one does not. Confirm "Nouvelle analyse" navigates back to `/app`.

- [ ] **Commit**

```bash
git add frontend/src/pages/HistoryPage.vue frontend/src/router.js
git commit -m "feat: add history page with mock data matching EvaluationHistoryDto"
```

---

# Phase 2 — Backend connection

---

## Task 5 — CORS (backend)

Without this, the browser blocks every request from port 5173 to port 5051.

**Files:**
- Modify: `backend/Program.cs`

- [ ] **Add CORS service** in the service configuration block, before `var app = builder.Build()`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
```

- [ ] **Apply CORS middleware** before `app.UseAuthentication()`:

```csharp
app.UseCors("FrontendDev");
```

- [ ] **Verify** — start the backend (`dotnet run`), open the browser console on `http://localhost:5173`, make a fetch to `http://localhost:5051/api/auth/login`, confirm no CORS error in the console.

- [ ] **Commit**

```bash
git add backend/Program.cs
git commit -m "feat: add CORS policy for local frontend dev"
```

---

## Task 6 — API service layer (frontend)

Shared fetch utility so every page uses the same base URL, token injection, and error handling.

**Files:**
- Create: `frontend/src/services/api.js`
- Create: `frontend/src/services/auth.js`
- Create: `frontend/src/services/evaluation.js`

- [ ] **Create `src/services/api.js`**:

```javascript
const BASE_URL = 'http://localhost:5051/api'

function getToken() {
  return localStorage.getItem('token')
}

export async function apiFetch(path, options = {}) {
  const token = getToken()
  const headers = {
    'Content-Type': 'application/json',
    ...(token ? { Authorization: `Bearer ${token}` } : {}),
    ...options.headers
  }
  const response = await fetch(`${BASE_URL}${path}`, { ...options, headers })
  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: response.statusText }))
    throw { status: response.status, ...error }
  }
  return response.json()
}
```

- [ ] **Create `src/services/auth.js`**:

```javascript
import { apiFetch } from './api.js'

export function login(email, password) {
  return apiFetch('/auth/login', {
    method: 'POST',
    body: JSON.stringify({ email, password })
  })
}

export function register(email, password, name, surname, companyName = null) {
  return apiFetch('/auth/register', {
    method: 'POST',
    body: JSON.stringify({ email, password, name, surname, companyName })
  })
}
```

- [ ] **Create `src/services/evaluation.js`**:

```javascript
import { apiFetch } from './api.js'

export function calculateImpact(payload) {
  return apiFetch('/Evaluation/calculate', {
    method: 'POST',
    body: JSON.stringify(payload)
  })
}

export function setAiScore(evaluationId, aiScore) {
  return apiFetch(`/Evaluation/${evaluationId}/score`, {
    method: 'PUT',
    body: JSON.stringify({ aiScore })
  })
}

export function getHistory(filters = {}) {
  const params = new URLSearchParams()
  if (filters.minCarbon != null) params.append('minCarbon', filters.minCarbon)
  if (filters.maxCarbon != null) params.append('maxCarbon', filters.maxCarbon)
  if (filters.aiScore) params.append('aiScore', filters.aiScore)
  if (filters.startDate) params.append('startDate', filters.startDate)
  if (filters.endDate) params.append('endDate', filters.endDate)
  const query = params.toString()
  return apiFetch(`/Evaluation/history${query ? `?${query}` : ''}`)
}
```

- [ ] **Commit**

```bash
git add frontend/src/services/
git commit -m "feat: add API service layer (auth + evaluation)"
```

---

## Task 7 — Connect login to real auth

Replace fake localStorage auth with real JWT. The backend expects `email`, not `username`.

**Files:**
- Modify: `frontend/src/pages/LoginPage.vue`
- Modify: `frontend/src/router.js`

- [ ] **Update the auth guard in `router.js`** — check `token` instead of `user`:

```javascript
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  if (to.meta.requiresAuth && !token) {
    next('/login')
  } else if (to.path === '/login' && token) {
    next('/app')
  } else {
    next()
  }
})
```

- [ ] **Replace the script block in `LoginPage.vue`**:

```javascript
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { login } from '../services/auth.js'

const router = useRouter()
const email = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

const handleLogin = async () => {
  if (!email.value.trim() || !password.value.trim()) {
    error.value = 'Veuillez remplir tous les champs'
    return
  }
  loading.value = true
  error.value = ''
  try {
    const data = await login(email.value, password.value)
    localStorage.setItem('token', data.token)
    router.push('/app')
  } catch {
    error.value = 'Email ou mot de passe incorrect'
  } finally {
    loading.value = false
  }
}
```

- [ ] **Update the template** — rename `username` field to `email`, add loading state, remove demo hint:

```html
<div class="form-group">
  <label for="email">Adresse email</label>
  <input
    id="email"
    v-model="email"
    type="email"
    placeholder="Entrez votre adresse email"
    @keyup.enter="handleLogin"
  >
</div>
<div class="form-group">
  <label for="password">Mot de passe</label>
  <input
    id="password"
    v-model="password"
    type="password"
    placeholder="Entrez votre mot de passe"
    @keyup.enter="handleLogin"
  >
</div>
<p v-if="error" class="error-message">{{ error }}</p>
<button type="submit" class="login-btn" :disabled="loading">
  {{ loading ? 'Connexion...' : 'Connexion' }}
</button>
```

- [ ] **Test** — log in with a real account from the database, confirm redirect to `/app` and token stored in localStorage. Try wrong credentials, confirm error message. Refresh the page, confirm the session persists.

- [ ] **Commit**

```bash
git add frontend/src/pages/LoginPage.vue frontend/src/router.js
git commit -m "feat: connect login to real JWT auth"
```

---

## Task 8 — Connect wizard to real API

Replace `mockSubmit` with a real call to `POST /api/Evaluation/calculate`.

**Files:**
- Modify: `frontend/src/pages/MainPage.vue`

- [ ] **Add the import** at the top of the script block:

```javascript
import { calculateImpact } from '../services/evaluation.js'
```

- [ ] **Replace `mockSubmit` and `handleSubmit`** with a real API call:

```javascript
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
```

- [ ] **Remove `mockSubmit`** — the function is no longer needed.

- [ ] **Test** — submit the wizard, confirm the network tab shows `POST /api/Evaluation/calculate` returning 200. Confirm results display real computed values. Test with `dataSensitivity=5` and `legalRisk=5` — confirm the backend rejects it and the error message appears.

- [ ] **Commit**

```bash
git add frontend/src/pages/MainPage.vue
git commit -m "feat: connect evaluation wizard to real backend API"
```

---

## Task 9 — Connect AI score rating to real API

Replace the no-op submit in `AIScoreRating.vue` with a real call to `PUT /api/Evaluation/{id}/score`.

**Files:**
- Modify: `frontend/src/components/AIScoreRating.vue`

- [ ] **Add import and real submit**:

```javascript
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
```

- [ ] **Update the template** — add loading state and error message:

```html
<button
  class="submit-rating-btn"
  type="button"
  :disabled="!selected || loading"
  @click="submitScore"
>
  {{ loading ? 'Enregistrement...' : 'Confirmer' }}
</button>
<p v-if="error" class="error-message">{{ error }}</p>
```

- [ ] **Test** — run an evaluation, select a score, confirm the network tab shows `PUT /api/Evaluation/1/score` with 200. Confirm the confirmation message appears. Check the database that the `AiScore` field was updated.

- [ ] **Commit**

```bash
git add frontend/src/components/AIScoreRating.vue
git commit -m "feat: connect AI score rating to real backend API"
```

---

## Task 10 — Connect history page to real API

Replace mock data with a real call to `GET /api/Evaluation/history`.

**Files:**
- Modify: `frontend/src/pages/HistoryPage.vue`

- [ ] **Add import and replace mock with real call**:

```javascript
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { getHistory } from '../services/evaluation.js'

const router = useRouter()
const history = ref([])
const loading = ref(true)
const error = ref('')

onMounted(async () => {
  try {
    history.value = await getHistory()
  } catch (err) {
    if (err.status === 401) {
      localStorage.removeItem('token')
      router.push('/login')
    } else {
      error.value = 'Impossible de charger l\'historique.'
    }
  } finally {
    loading.value = false
  }
})
```

- [ ] **Test** — run two evaluations, navigate to `/history`, confirm both entries appear sorted by date (most recent first). Confirm the AI score badge shows only when `aiScore` is non-empty.

- [ ] **Commit**

```bash
git add frontend/src/pages/HistoryPage.vue
git commit -m "feat: connect history page to real backend API"
```

---

# Phase 3 — Cleanup

---

## Task 11 — Translate backend French comments to English

**Files:**
- Modify: `backend/Program.cs`
- Modify: `backend/Controllers/AuthController.cs`
- Modify: `backend/Controllers/EvaluationController.cs`
- Modify: `backend/Services/AuthService.cs`
- Modify: `backend/Services/EvaluationService.cs`
- Modify: `backend/Services/ImpactCalculator.cs`

Translate all French comments to English. UI-facing strings returned in API responses (error messages) may stay in French.

- [ ] **Translate all comments** in the 6 files above.

- [ ] **Commit**

```bash
git add backend/
git commit -m "chore: translate backend comments to English"
```

---

## Execution order

```
Phase 1 (frontend UI):
  Task 1 → Task 2 → Task 3 → Task 4

Phase 2 (backend connection):
  Task 5 → Task 6 → Task 7 → Task 8 → Task 9 → Task 10

Phase 3 (cleanup):
  Task 11
```

Do not start Phase 2 until Phase 1 is complete and visually verified.
