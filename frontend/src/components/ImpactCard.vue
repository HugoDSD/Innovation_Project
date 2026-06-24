<script setup>
defineProps({
  title: {
    type: String,
    required: true
  },
  value: {
    type: String,
    default: '-'
  },
  percentage: {
    type: Number,
    default: 0
  },
  color: {
    type: String,
    default: '#667eea'
  }
})
</script>

<template>
  <div class="impact-card" :style="{ borderTopColor: color }">
    <h4 class="card-title">{{ title }}</h4>
    <div class="card-content">
      <div class="percentage-display" :style="{ color }">
        <span class="percentage-value">{{ percentage }}%</span>
      </div>
      <div class="value-section">
        <p class="value-label">Analyse:</p>
        <p class="value-text">{{ value }}</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.impact-card {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  border-top: 4px solid #667eea;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.impact-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
}

.card-title {
  margin: 0 0 1rem 0;
  color: #333;
  font-size: 1.1rem;
  font-weight: 600;
}

.card-content {
  display: flex;
  align-items: center;
  gap: 1.5rem;
}

.percentage-display {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.percentage-value {
  font-size: 2.5rem;
  font-weight: bold;
}

.value-section {
  flex: 1;
}

.value-label {
  margin: 0 0 0.5rem 0;
  color: #666;
  font-size: 0.9rem;
  font-weight: 500;
}

.value-text {
  margin: 0;
  color: #333;
  font-size: 1rem;
  line-height: 1.4;
}
</style>
