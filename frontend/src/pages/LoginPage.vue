<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import apiService from '../services/apiService'

const router = useRouter()
const isRegistering = ref(false)

const loginForm = ref({ email: '', password: '' })
const registerForm = ref({ email: '', password: '', name: '', surname: '', companyName: '' })

const error = ref('')
const success = ref('')
const loading = ref(false)

const TEST_EMAIL = 'test@test.com'
const TEST_PASSWORD = 'test'

const handleLogin = async () => {
  error.value = ''
  if (!loginForm.value.email.trim() || !loginForm.value.password.trim()) {
    error.value = 'Veuillez remplir tous les champs'
    return
  }
  // Compte de test local (sans backend)
  if (loginForm.value.email === TEST_EMAIL && loginForm.value.password === TEST_PASSWORD) {
    apiService.setToken('test-token-local')
    router.push('/app')
    return
  }
  loading.value = true
  try {
    await apiService.login(loginForm.value.email, loginForm.value.password)
    router.push('/app')
  } catch (e) {
    error.value = e.message || 'Connexion échouée'
  } finally {
    loading.value = false
  }
}

const handleRegister = async () => {
  error.value = ''
  success.value = ''
  const { email, password, name, surname, companyName } = registerForm.value
  if (!email.trim() || !password.trim() || !name.trim() || !surname.trim() || !companyName.trim()) {
    error.value = 'Veuillez remplir tous les champs'
    return
  }
  loading.value = true
 try {
    // On envoie les variables, le service s'occupe du formatage JSON
    await apiService.register(email, password, name, surname, companyName)
    success.value = 'Compte créé avec succès.'
    isRegistering.value = false
  } catch (e) {
    error.value = e.message || 'Inscription échouée'
  } finally {
    loading.value = false
  }
}

const switchMode = () => {
  isRegistering.value = !isRegistering.value
  error.value = ''
  success.value = ''
}
</script>

<template>
  <div class="login-container">
    <div class="login-box">
      <h1>EcoIA Évaluateur</h1>
      <p class="subtitle">{{ isRegistering ? 'Créer un compte' : 'Connectez-vous pour commencer' }}</p>

      <form v-if="!isRegistering" @submit.prevent="handleLogin">
        <div class="form-group">
          <label>Email</label>
          <input v-model="loginForm.email" type="email" placeholder="votre@email.com" @keyup.enter="handleLogin">
        </div>
        <div class="form-group">
          <label>Mot de passe</label>
          <input v-model="loginForm.password" type="password" placeholder="••••••••" @keyup.enter="handleLogin">
        </div>
        <p v-if="success" class="success-message">{{ success }}</p>
        <p v-if="error" class="error-message">{{ error }}</p>
        <button type="submit" class="submit-btn" :disabled="loading">
          {{ loading ? 'Connexion...' : 'Connexion' }}
        </button>
      </form>

      <form v-else @submit.prevent="handleRegister">
        <div class="form-row">
          <div class="form-group">
            <label>Prénom</label>
            <input v-model="registerForm.name" type="text" placeholder="Votre prénom">
          </div>
          <div class="form-group">
            <label>Nom</label>
            <input v-model="registerForm.surname" type="text" placeholder="Votre nom">
          </div>
        </div>
        <div class="form-group">
          <label>Email</label>
          <input v-model="registerForm.email" type="email" placeholder="votre@email.com">
        </div>
        <div class="form-group">
          <label>Mot de passe</label>
          <input v-model="registerForm.password" type="password" placeholder="••••••••">
        </div>
        <div class="form-group">
          <label>Entreprise</label>
          <input v-model="registerForm.companyName" type="text" placeholder="Votre entreprise">
        </div>
        <p v-if="error" class="error-message">{{ error }}</p>
        <button type="submit" class="submit-btn" :disabled="loading">
          {{ loading ? 'Inscription...' : 'Créer un compte' }}
        </button>
      </form>

      <p class="switch-link">
        {{ isRegistering ? 'Déjà un compte ?' : 'Pas encore de compte ?' }}
        <button type="button" class="link-btn" @click="switchMode">
          {{ isRegistering ? 'Se connecter' : "S'inscrire" }}
        </button>
      </p>

      <p v-if="!isRegistering" class="demo-hint">
        Compte test (sans backend) — <strong>test@test.com</strong> / <strong>test</strong>
      </p>
    </div>
  </div>
</template>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background-color: darkblue;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

.login-box {
  background: white;
  padding: 2.5rem;
  border-radius: 10px;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
  width: 100%;
  max-width: 420px;
}

h1 {
  text-align: center;
  color: #333;
  margin-bottom: 0.5rem;
  font-size: 1.8rem;
}

.subtitle {
  text-align: center;
  color: #666;
  margin-bottom: 2rem;
  font-size: 0.95rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 0.75rem;
}

.form-group {
  margin-bottom: 1.25rem;
  display: flex;
  flex-direction: column;
}

label {
  display: block;
  margin-bottom: 0.4rem;
  color: #333;
  font-weight: 500;
  font-size: 0.9rem;
}

input {
  width: 100%;
  padding: 0.7rem;
  border: 2px solid #ddd;
  border-radius: 5px;
  font-size: 1rem;
  transition: border-color 0.3s;
  box-sizing: border-box;
}

input:focus {
  outline: none;
  border-color: #667eea;
  box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
}

.success-message {
  color: #27ae60;
  font-size: 0.9rem;
  margin-bottom: 1rem;
  text-align: center;
  background: #d4edda;
  padding: 0.5rem;
  border-radius: 4px;
}

.error-message {
  color: #e74c3c;
  font-size: 0.9rem;
  margin-bottom: 1rem;
  text-align: center;
  background: #f8d7da;
  padding: 0.5rem;
  border-radius: 4px;
}

.submit-btn {
  width: 100%;
  padding: 0.8rem;
  background-color: darkblue;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: opacity 0.2s;
  margin-top: 0.5rem;
}

.submit-btn:hover:not(:disabled) {
  opacity: 0.88;
}

.submit-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.switch-link {
  text-align: center;
  color: #666;
  font-size: 0.9rem;
  margin-top: 1.5rem;
  margin-bottom: 0;
}

.link-btn {
  background: none;
  border: none;
  color: darkblue;
  font-weight: 600;
  cursor: pointer;
  font-size: 0.9rem;
  padding: 0;
  text-decoration: underline;
}

.link-btn:hover {
  opacity: 0.75;
}

.demo-hint {
  text-align: center;
  color: #888;
  font-size: 0.8rem;
  margin-top: 1rem;
  margin-bottom: 0;
  padding: 0.5rem;
  background: #f8f9fa;
  border-radius: 4px;
}
</style>
