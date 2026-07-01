<script setup>
defineProps({
  carbonFootprint: {
    type: Number,
    required: true
  },
  emissions: {
    type: Array,
    default: () => []
  }
})
</script>

<template>
  <div class="environmental-card">
    <h4 class="card-title">Impact Environnemental</h4>
    
    <div class="carbon-section">
      <div class="carbon-display">
        <span class="carbon-value">{{ carbonFootprint }}</span>
        <span class="carbon-unit">kg CO₂</span>
      </div>
      <p class="carbon-label">Empreinte carbone estimée</p>
    </div>

    <div class="emissions-detail">
      <h5>Détail par étapes du projet</h5>
      <div class="emissions-list">
        <div v-for="(emission, index) in emissions" :key="index" class="emission-item">
          <div class="emission-header">
            <span class="emission-stage">{{ emission.stage }}</span>
            <span class="emission-amount">{{ emission.amount }} kg CO₂</span>
          </div>
          <p class="emission-description">{{ emission.description }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.environmental-card {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  border-top: 4px solid #27ae60;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.card-title {
  margin: 0 0 1.5rem 0;
  color: #333;
  font-size: 1.1rem;
  font-weight: 600;
}

.carbon-section {
  background-color: rgba(39, 174, 96, 0.1);
  padding: 1.5rem;
  border-radius: 8px;
  text-align: center;
  margin-bottom: 1.5rem;
}

.carbon-display {
  display: flex;
  align-items: baseline;
  justify-content: center;
  gap: 0.5rem;
  margin-bottom: 0.5rem;
}

.carbon-value {
  font-size: 2.5rem;
  font-weight: bold;
  color: #27ae60;
}

.carbon-unit {
  font-size: 1.1rem;
  color: #27ae60;
  font-weight: 600;
}

.carbon-label {
  margin: 0;
  color: #666;
  font-size: 0.9rem;
}

.emissions-detail {
  margin-top: 1.5rem;
}

.emissions-detail h5 {
  margin: 0 0 1rem 0;
  color: #333;
  font-size: 0.95rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.emissions-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.emission-item {
  background: #f8f9fa;
  padding: 1rem;
  border-radius: 6px;
  border-left: 3px solid #27ae60;
}

.emission-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 0.5rem;
}

.emission-stage {
  font-weight: 600;
  color: #333;
  font-size: 0.95rem;
}

.emission-amount {
  color: #27ae60;
  font-weight: 600;
  font-size: 0.9rem;
}

.emission-description {
  margin: 0;
  color: #666;
  font-size: 0.85rem;
  line-height: 1.4;
}
</style>
