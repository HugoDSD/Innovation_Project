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
