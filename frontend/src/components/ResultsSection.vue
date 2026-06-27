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

<style scoped>
.results-section {
  animation: slideUp 0.5s ease;
  margin-top: 3rem;
  padding-top: 2rem;
  border-top: 2px solid #eee;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.results-title {
  text-align: center;
  color: #333;
  margin-bottom: 2rem;
  font-size: 1.5rem;
}

.impact-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin-bottom: 2rem;
}

.detailed-analysis {
  background: #f8f9fa;
  padding: 1.5rem;
  border-radius: 8px;
  border-left: 4px solid #667eea;
}

.detailed-analysis h3 {
  margin-top: 0;
  color: #333;
}

.detailed-analysis p {
  color: #666;
  line-height: 1.6;
}

@media (max-width: 768px) {
  .impact-grid {
    grid-template-columns: 1fr;
  }
}
</style>
