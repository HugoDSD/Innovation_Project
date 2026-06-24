<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import ResultsSection from '../components/ResultsSection.vue'

const router = useRouter()

const formData = ref({
  projectName: '',
  projectDescription: '',
  duration: '',
  teamSize: '',
  budget: '',
  complexity: '',
  deadline: '',
  technology: ''
})

const results = ref(null)
const showResults = ref(false)

// Données aléatoires pour simuler une réponse
const generateRandomResults = () => {
  const levels = ['none', 'not_recommended', 'recommended', 'highly_recommended']
  const randomLevel = levels[Math.floor(Math.random() * levels.length)]
  const levelIndex = levels.indexOf(randomLevel)

  // Données pour empreinte carbone
  const carbonFootprints = [150, 280, 450, 650]
  const emissions = [
    [
      { stage: 'Infrastructure', amount: 50, description: 'Hébergement et serveurs' },
      { stage: 'Développement', amount: 60, description: 'Ressources de développement' },
      { stage: 'Maintenance', amount: 40, description: 'Maintenance et support' }
    ],
    [
      { stage: 'Infrastructure', amount: 90, description: 'Hébergement et serveurs cloud' },
      { stage: 'Développement', amount: 120, description: 'Ressources de développement et tests' },
      { stage: 'Maintenance', amount: 70, description: 'Maintenance et support continu' }
    ],
    [
      { stage: 'Infrastructure', amount: 150, description: 'Infrastructure optimisée' },
      { stage: 'Développement', amount: 200, description: 'Développement avec IA' },
      { stage: 'Maintenance', amount: 100, description: 'Maintenance automatisée' }
    ],
    [
      { stage: 'Infrastructure', amount: 200, description: 'Infrastructure très optimisée' },
      { stage: 'Développement', amount: 300, description: 'Développement accéléré par IA' },
      { stage: 'Maintenance', amount: 150, description: 'Maintenance préventive automatisée' }
    ]
  ]

  // Recommendations techniques
  const technicalRecommendations = [
    [
      'Utiliser des méthodes traditionnelles de développement',
      'Optimiser l\'infrastructure existante',
      'Mettre en place un monitoring performant'
    ],
    [
      'Considérer une automatisation partielle',
      'Optimiser les pipelines de déploiement',
      'Mettre en place des tests automatisés'
    ],
    [
      'Intégrer des outils d\'IA pour l\'automatisation',
      'Utiliser le machine learning pour l\'optimisation',
      'Implémenter des processus intelligents'
    ],
    [
      'Déployer une IA générative pour la génération de code',
      'Utiliser l\'IA pour l\'optimisation complète',
      'Automatiser tous les processus répétitifs'
    ]
  ]

  // Alternatives sans IA
  const alternatives = [
    [
      'Scripts bash et outils shell classiques',
      'Équipe dédiée pour les tâches manuelles',
      'Approche itérative sans automatisation'
    ],
    [
      'Outils d\'automatisation traditionnels (Jenkins, GitLab CI)',
      'Processus manuels documentés',
      'Tests effectués par l\'équipe QA'
    ],
    [
      'Templating et frameworks existants',
      'Configuration manuelle des environnements',
      'Analyse manuelle des performances'
    ],
    [
      'Développement entièrement manuel avec équipe expérimentée',
      'Patterns et bonnes pratiques documentées',
      'Revues de code systématiques'
    ]
  ]

  // Actions pour réduire l'impact
  const actions = [
    [
      'Maintenir une bonne documentation',
      'Former l\'équipe régulièrement',
      'Planifier avec soin'
    ],
    [
      'Automatiser les tâches répétitives',
      'Utiliser le CI/CD efficacement',
      'Réduire les inefficacités manuelles'
    ],
    [
      'Mettre en place l\'IA pour l\'optimisation',
      'Analyser les performances en continu',
      'Automatiser les processus critiques'
    ],
    [
      'Maximiser l\'utilisation de l\'IA générative',
      'Implémenter le self-healing automatique',
      'Optimiser les ressources en temps réel'
    ]
  ]

  const detailedAnalysisTexts = [
    'Votre projet n\'a pas besoin d\'IA pour être réussi. Les méthodes traditionnelles suffisent.',
    'L\'IA pourrait aider mais n\'est pas essentielle pour ce projet.',
    'L\'IA est recommandée pour optimiser ce projet et améliorer les résultats.',
    'L\'IA est fortement recommandée pour maximiser l\'efficacité et l\'impact de votre projet.'
  ]

  return {
    aiLevel: randomLevel,
    carbonFootprint: carbonFootprints[levelIndex],
    emissions: emissions[levelIndex],
    technicalRecommendations: technicalRecommendations[levelIndex],
    alternativesWithoutAI: alternatives[levelIndex],
    actionItems: actions[levelIndex],
    detailedAnalysis: detailedAnalysisTexts[levelIndex]
  }
}

const handleSubmit = () => {
  if (validateForm()) {
    results.value = generateRandomResults()
    showResults.value = true
    
    // Faire défiler vers les résultats après un court délai
    setTimeout(() => {
      document.querySelector('.results-section')?.scrollIntoView({ behavior: 'smooth' })
    }, 100)
  }
}

const validateForm = () => {
  return formData.value.projectName.trim() &&
         formData.value.projectDescription.trim() &&
         formData.value.duration &&
         formData.value.teamSize &&
         formData.value.complexity
}

const handleLogout = () => {
  localStorage.removeItem('user')
  router.push('/login')
}

const resetForm = () => {
  formData.value = {
    projectName: '',
    projectDescription: '',
    duration: '',
    teamSize: '',
    budget: '',
    complexity: '',
    deadline: '',
    technology: ''
  }
  showResults.value = false
  results.value = null
}
</script>

<template>
  <div class="main-container">
    <header class="header">
      <div class="header-content">
        <h1>Je ne sais pas quoi mettre comme titre :-)</h1>
        <button class="logout-btn" @click="handleLogout">Déconnexion</button>
      </div>
    </header>

    <div class="content">
      <div class="form-container">
        <h2 class="form-title">Analysez votre projet</h2>
        <p class="form-subtitle">
            Remplissez les informations ci-dessous pour obtenir une analyse d'IA <br>
            Les champs dotés d'un * sont obligatoires pour lancer l'analyse
        </p>

        <form @submit.prevent="handleSubmit" class="project-form">
          <div class="form-row">
            <div class="form-group">
              <label for="projectName">Nom du projet *</label>
              <input
                id="projectName"
                v-model="formData.projectName"
                type="text"
                placeholder="Ex: Plateforme e-commerce"
                required
              >
            </div>
            <div class="form-group">
              <label for="complexity">Complexité *</label>
              <select id="complexity" v-model="formData.complexity" required>
                <option value="">Sélectionnez...</option>
                <option value="low">Faible</option>
                <option value="medium">Moyenne</option>
                <option value="high">Élevée</option>
                <option value="very-high">Très élevée</option>
              </select>
            </div>
          </div>

          <div class="form-group full-width">
            <label for="description">Description du projet *</label>
            <textarea
              id="description"
              v-model="formData.projectDescription"
              placeholder="Décrivez votre projet en détail..."
              rows="4"
              required
            ></textarea>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="duration">Durée estimée *</label>
              <select id="duration" v-model="formData.duration" required>
                <option value="">Sélectionnez...</option>
                <option value="1-3-months">1-3 mois</option>
                <option value="3-6-months">3-6 mois</option>
                <option value="6-12-months">6-12 mois</option>
                <option value="12-plus-months">12+ mois</option>
              </select>
            </div>
            <div class="form-group">
              <label for="teamSize">Taille de l'équipe *</label>
              <select id="teamSize" v-model="formData.teamSize" required>
                <option value="">Sélectionnez...</option>
                <option value="1-3">1-3 personnes</option>
                <option value="4-10">4-10 personnes</option>
                <option value="11-20">11-20 personnes</option>
                <option value="20-plus">20+ personnes</option>
              </select>
            </div>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="budget">Budget</label>
              <select id="budget" v-model="formData.budget">
                <option value="">Sélectionnez...</option>
                <option value="null">Nul (0€)</option>
                <option value="low">Faible (1-10k€)</option>
                <option value="medium">Moyen (10k-50k€)</option>
                <option value="high">Élevé (50k-200k€)</option>
                <option value="very-high">Très élevé (plus de 200k€)</option>
              </select>
            </div>
            <div class="form-group">
              <label for="technology">Stack technologique</label>
              <input
                id="technology"
                v-model="formData.technology"
                type="text"
                placeholder="Ex: Vue.js, Node.js, PostgreSQL"
              >
            </div>
          </div>

          <div class="form-group">
            <label for="deadline">Deadline</label>
            <input
              id="deadline"
              v-model="formData.deadline"
              type="date"
            >
          </div>

          <div class="form-actions">
            <button type="submit" class="submit-btn">Analyser le projet</button>
            <button type="button" class="reset-btn" @click="resetForm">Réinitialiser</button>
          </div>
        </form>
      </div>

      <ResultsSection
        v-if="showResults"
        :results="results"
        :show-results="showResults"
        class="results-section"
      />
    </div>
  </div>
</template>

<style scoped>
.main-container {
  min-height: 100vh;
  background-color: lightgrey;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.header {
  background-color: darkblue;
  color: white;
  padding: 1.5rem 0;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  position: sticky;
  top: 0;
  z-index: 100;
}

.header-content {
  max-width: 900px;
  margin: 0 auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 1.5rem;
}

.header h1 {
  margin: 0;
  font-size: 1.8rem;
}

.logout-btn {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 2px solid white;
  padding: 0.5rem 1rem;
  border-radius: 5px;
  cursor: pointer;
  font-weight: 600;
  transition: all 0.3s;
}

.logout-btn:hover {
  background: rgba(255, 255, 255, 0.3);
}

.content {
  max-width: 1000px;
  margin: 2rem auto;
  padding: 0 1.5rem;
}

.form-container {
  background: white;
  padding: 2rem;
  border-radius: 10px;
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.1);
}

.form-title {
  color: #333;
  font-size: 1.6rem;
  margin-top: 0;
  margin-bottom: 0.5rem;
  text-align: center;
}

.form-subtitle {
  text-align: center;
  color: #666;
  margin-bottom: 1.5rem;
}

.project-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-group {
  display: flex;
  flex-direction: column;
}

.form-group.full-width {
  grid-column: 1 / -1;
}

label {
  margin-bottom: 0.5rem;
  color: #333;
  font-weight: 500;
  font-size: 0.95rem;
}

input,
select,
textarea {
  padding: 0.75rem;
  border: 2px solid #ddd;
  border-radius: 5px;
  font-size: 1rem;
  font-family: inherit;
  transition: border-color 0.3s;
}

input:focus,
select:focus,
textarea:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

textarea {
  resize: vertical;
}

.form-actions {
  display: flex;
  gap: 1rem;
  justify-content: center;
  margin-top: 1rem;
}

.submit-btn {
  padding: 0.875rem 2rem;
  background-color: darkblue;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.submit-btn:hover {
  box-shadow: 0 5px 20px rgba(102, 126, 234, 0.4);
}

.reset-btn {
  padding: 0.875rem 2rem;
  background: #f0f0f0;
  color: #333;
  border: 2px solid #ddd;
  border-radius: 5px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.reset-btn:hover {
  background: #e8e8e8;
  border-color: #999;
}

@media (max-width: 600px) {
  .form-row {
    grid-template-columns: 1fr;
  }

  .form-actions {
    flex-direction: column;
  }

  .submit-btn,
  .reset-btn {
    width: 100%;
  }

  .header-content {
    flex-direction: column;
    gap: 1rem;
  }

  .header h1 {
    font-size: 1.5rem;
  }
}
</style>
