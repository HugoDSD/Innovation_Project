<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import apiService from '../services/apiService'

const router = useRouter()
const isRegistering = ref(false)

const loginForm = ref({ email: '', password: '' })
const registerForm = ref({
  email: '',
  password: '',
  name: '',
  surname: '',
  companyName: ''
})

const error = ref('')
const success = ref('')
const loading = ref(false)

const handleLogin = async () => {
  error.value = ''
  success.value = ''

  if (!loginForm.value.email.trim() || !loginForm.value.password.trim()) {
    error.value = 'Veuillez remplir tous les champs'
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
      <h1>SobrIA</h1>
      <p class="subtitle">
        {{ isRegistering ? 'Créer un compte' : 'Connectez-vous pour commencer' }}
      </p>

      <form v-if="!isRegistering" @submit.prevent="handleLogin">
        <div class="form-group">
          <label>Email</label>
          <input v-model="loginForm.email" type="email" placeholder="votre@email.com">
        </div>

        <div class="form-group">
          <label>Mot de passe</label>
          <input v-model="loginForm.password" type="password" placeholder="••••••••">
        </div>

        <p v-if="error" class="error-message">{{ error }}</p>

        <button type="submit" class="submit-btn" :disabled="loading">
          {{ loading ? 'Connexion...' : 'Connexion' }}
        </button>
      </form>

      <form v-else @submit.prevent="handleRegister">
        <div class="form-group">
          <label>Prénom</label>
          <input v-model="registerForm.name" type="text">
        </div>

        <div class="form-group">
          <label>Nom</label>
          <input v-model="registerForm.surname" type="text">
        </div>

        <div class="form-group">
          <label>Email</label>
          <input v-model="registerForm.email" type="email">
        </div>

        <div class="form-group">
          <label>Mot de passe</label>
          <input v-model="registerForm.password" type="password">
        </div>

        <div class="form-group">
          <label>Entreprise</label>
          <input v-model="registerForm.companyName" type="text">
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
    </div>
  </div>
</template>

<style scoped>
.login-container {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  background: linear-gradient(135deg, #0f2f28, #1f5a4a, #3a8d6d);
  background-size: 400% 400%;
  animation: gradientMove 12s ease infinite;
}

@keyframes gradientMove {
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
}

.login-box {
  background: rgba(255, 255, 255, 0.12);
  padding: 3.5rem;
  border-radius: 18px;
  width: 100%;
  max-width: 520px;
  box-shadow: 0 25px 70px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(255, 255, 255, 0.18);
}

h1 {
  text-align: center;
  color: white;
  font-size: 2.3rem;
}

.subtitle {
  text-align: center;
  color: rgba(255,255,255,0.85);
  margin-bottom: 2rem;
}

.form-group {
  margin-bottom: 1.2rem;
}

label {
  color: #eaf7f1;
  display: block;
  margin-bottom: 0.4rem;
}

input {
  width: 100%;
  padding: 0.9rem;
  border-radius: 12px;
  border: 1px solid rgba(255,255,255,0.25);
  background: rgba(255,255,255,0.08);
  color: white;
}

input:focus {
  outline: none;
  border-color: #58c79a;
}

.submit-btn {
  width: 100%;
  padding: 1rem;
  border-radius: 12px;
  border: none;
  background: linear-gradient(135deg, #3fbf8f, #2f9d74);
  color: white;
  font-weight: 600;
  cursor: pointer;
}

.error-message {
  color: #ffb4a9;
  margin-top: 0.5rem;
}

.switch-link {
  text-align: center;
  margin-top: 1.5rem;
  color: white;
}

.link-btn {
  background: none;
  border: none;
  color: #58c79a;
  cursor: pointer;
}
</style>