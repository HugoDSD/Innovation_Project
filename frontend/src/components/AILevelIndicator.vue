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
