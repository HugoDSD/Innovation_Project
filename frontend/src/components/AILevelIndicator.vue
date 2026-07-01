<script setup>
const props = defineProps({
  level: {
    type: String,
    default: null,
    validator: (value) => !value || ['none', 'not_recommended', 'recommended', 'highly_recommended'].includes(value)
  }
})

const levels = [
  { key: 'none', label: 'Pas d\'IA', color: '#e8e8e8', darkColor: '#333' },
  { key: 'not_recommended', label: 'IA non recommandée', color: '#f8d7da', darkColor: '#721c24' },
  { key: 'recommended', label: 'IA recommandée', color: '#d4edda', darkColor: '#155724' },
  { key: 'highly_recommended', label: 'IA très recommandée', color: '#cfe2ff', darkColor: '#084298' }
]

const isActive = (key) => props.level === key
</script>

<template>
  <div class="ai-level-container">
    <h3 class="title">Niveau d'utilité de l'IA</h3>
    <div class="level-grid">
      <div
        v-for="item in levels"
        :key="item.key"
        class="level-item"
        :class="{ active: isActive(item.key) }"
        :style="isActive(item.key) ? {
          backgroundColor: item.color,
          color: item.darkColor,
          boxShadow: `0 0 15px ${item.darkColor}40`
        } : {
          backgroundColor: item.color,
          color: item.darkColor,
          opacity: 0.5
        }"
      >
        <span class="level-text">{{ item.label }}</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.ai-level-container {
  margin-bottom: 2rem;
}

.title {
  text-align: center;
  color: #333;
  margin-bottom: 1rem;
  font-size: 1.2rem;
}

.level-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
}

.level-item {
  padding: 1.5rem;
  border-radius: 8px;
  text-align: center;
  transition: all 0.3s ease;
  cursor: default;
  border: 3px solid transparent;
}

.level-item.active {
  border-color: currentColor;
  transform: scale(1.05);
  font-weight: 600;
}

.level-text {
  display: block;
  font-size: 0.95rem;
}

@media (max-width: 600px) {
  .level-grid {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
