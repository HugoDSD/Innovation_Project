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
