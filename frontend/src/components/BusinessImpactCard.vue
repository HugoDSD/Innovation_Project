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
