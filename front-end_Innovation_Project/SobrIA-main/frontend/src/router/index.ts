import { createRouter, createWebHistory } from 'vue-router'
import { useEvaluation } from '@/stores/evaluation'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'form',
      component: () => import('@/views/FormView.vue'),
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginPage.vue'),
    },
    {
      path: '/simulation',
      name: 'simulation',
      component: () => import('@/views/SimulatorView.vue'),
      meta: { requiresInput: true },
    },
    {
      path: '/resultat',
      name: 'result',
      component: () => import('@/views/ResultView.vue'),
      meta: { requiresInput: true },
    },
    {
      path: '/historique',
      name: 'history',
      component: () => import('@/views/HistoryView.vue'),
    },
    {
      path: '/historique/:id',
      name: 'history-detail',
      component: () => import('@/views/ResultView.vue'),
      props: true,
    },
    { path: '/:pathMatch(.*)*', redirect: '/' },
  ],
  scrollBehavior() {
    return { top: 0 }
  },
})

// Guard: simulation/result need a captured workflow, else send back to the form.
  router.beforeEach((to) => {
  const isAuthenticated = !!localStorage.getItem('token') // On vérifie le token
  
  if (to.name !== 'login' && !isAuthenticated) {
    return { name: 'login' }
  }
  
  if (to.meta.requiresInput && !useEvaluation().state.input) {
    return { name: 'form' }
  }
  return true
})

export default router
