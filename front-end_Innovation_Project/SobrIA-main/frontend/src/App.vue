<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import AppStepper from '@/components/AppStepper.vue'
</script>

<template>
  <div class="app-grain min-h-screen flex flex-col">
    <header class="border-b border-forest-200/60 bg-paper/80 backdrop-blur sticky top-0 z-20">
      <div class="mx-auto max-w-5xl px-5 h-16 flex items-center justify-between">
        <RouterLink to="/" class="flex items-baseline gap-2 group">
          <span class="font-display text-2xl font-700 text-forest-700 tracking-tight">
            Sobr<span class="text-forest-500">IA</span>
          </span>
          <span class="hidden sm:inline text-xs text-ink-400 group-hover:text-ink-500 transition-colors">
            l'IA avec sobriété
          </span>
        </RouterLink>

        <nav class="flex items-center gap-1 text-sm">
          <RouterLink
            to="/"
            class="px-3 py-1.5 rounded-full text-ink-500 hover:text-forest-700 hover:bg-forest-50 transition-colors"
            active-class="!text-forest-700 !bg-forest-100"
          >
            Évaluer
          </RouterLink>
          <RouterLink
            to="/historique"
            class="px-3 py-1.5 rounded-full text-ink-500 hover:text-forest-700 hover:bg-forest-50 transition-colors"
            active-class="!text-forest-700 !bg-forest-100"
          >
            Historique
          </RouterLink>
        </nav>
      </div>
    </header>

    <AppStepper />

    <main class="flex-1">
      <RouterView v-slot="{ Component }">
        <transition name="fade" mode="out-in">
          <component :is="Component" />
        </transition>
      </RouterView>
    </main>

    <footer class="border-t border-forest-200/50 mt-12">
      <div class="mx-auto max-w-5xl px-5 py-6 text-xs text-ink-400 flex flex-wrap gap-x-4 gap-y-1 justify-between">
        <span>SobrIA — prototype Phase 1 · données simulées (stubs aléatoires)</span>
        <span>Notation 1–5 et verdict : logique réelle</span>
      </div>
    </footer>
  </div>
</template>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.2s ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}
</style>
