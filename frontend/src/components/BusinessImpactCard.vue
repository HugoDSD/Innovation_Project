<script setup>
const props = defineProps({
  recommendations: {
    type: Array,
    default: () => []
  },
  alternatives: {
    type: Array,
    default: () => []
  },
  actionItems: {
    type: Array,
    default: () => []
  }
})
</script>

<template>
  <div class="business-card">
    <h4 class="card-title">Impact Métier</h4>

    <div class="section">
      <h5 class="section-title">
        <span class="icon">💡</span>
        Recommandations techniques
      </h5>
      <ul class="list">
        <li v-for="(rec, index) in props.recommendations" :key="`rec-${index}`">
          <span class="list-marker">✓</span>
          <span>{{ rec }}</span>
        </li>
      </ul>
    </div>

    <div class="section">
      <h5 class="section-title">
        <span class="icon">🔄</span>
        Alternatives sans IA
      </h5>
      <ul class="list">
        <li v-for="(alt, index) in props.alternatives" :key="`alt-${index}`">
          <span class="list-marker">→</span>
          <span>{{ alt }}</span>
        </li>
      </ul>
    </div>

    <div class="section">
      <h5 class="section-title">
        <span class="icon">🎯</span>
        Actions pour réduire l'impact
      </h5>
      <ul class="list action-list">
        <li v-for="(action, index) in props.actionItems" :key="`action-${index}`">
          <span class="action-badge">{{ index + 1 }}</span>
          <span>{{ action }}</span>
        </li>
      </ul>
    </div>
  </div>
</template>

<style scoped>
.business-card {
  background: white;
  border-radius: 8px;
  padding: 1.5rem;
  border-top: 4px solid #267bf1;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.card-title {
  margin: 0 0 1.5rem 0;
  color: #333;
  font-size: 1.1rem;
  font-weight: 600;
}

.section {
  margin-bottom: 1.5rem;
}

.section:last-child {
  margin-bottom: 0;
}

.section-title {
  margin: 0 0 0.75rem 0;
  color: #333;
  font-size: 0.95rem;
  font-weight: 600;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.icon {
  font-size: 1.1rem;
}

.list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.list li {
  display: flex;
  align-items: flex-start;
  gap: 0.75rem;
  color: #666;
  font-size: 0.9rem;
  line-height: 1.4;
}

.list-marker {
  color: #267bf1;
  font-weight: 600;
  flex-shrink: 0;
  min-width: 1.2rem;
}

.action-list li {
  background: #f8f9fa;
  padding: 0.75rem;
  border-radius: 6px;
  border-left: 3px solid #267bf1;
}

.action-badge {
  background: #267bf1;
  color: white;
  border-radius: 50%;
  width: 1.5rem;
  height: 1.5rem;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8rem;
  font-weight: bold;
  flex-shrink: 0;
}
</style>
