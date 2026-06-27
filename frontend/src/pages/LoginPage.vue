<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const username = ref('')
const password = ref('')
const error = ref('')

const handleLogin = () => {
  if (username.value.trim() && password.value.trim()) {
    // Simulate authentication
    localStorage.setItem('user', JSON.stringify({
      username: username.value,
      timestamp: new Date().toISOString()
    }))
    router.push('/app')
  } else {
    error.value = 'Veuillez remplir tous les champs'
  }
}
</script>

<template>
  <div class="login-container">
    <div class="login-box">
      <h1>Analyse d'IA pour Projets</h1>
      <p class="subtitle">Connectez-vous pour commencer</p>
      
      <form @submit.prevent="handleLogin">
        <div class="form-group">
          <label for="username">Nom d'utilisateur</label>
          <input 
            id="username"
            v-model="username" 
            type="text" 
            placeholder="Entrez votre nom d'utilisateur"
            @keyup.enter="handleLogin"
          >
        </div>

        <div class="form-group">
          <label for="password">Mot de passe</label>
          <input 
            id="password"
            v-model="password" 
            type="password" 
            placeholder="Entrez votre mot de passe"
            @keyup.enter="handleLogin"
          >
        </div>

        <p v-if="error" class="error-message">{{ error }}</p>

        <button type="submit" class="login-btn">Connexion</button>
      </form>

      <p class="demo-hint">Pour démo: Utilisez n'importe quel identifiant et mot de passe</p>
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
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.2);
  width: 100%;
  max-width: 400px;
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

.form-group {
  margin-bottom: 1.5rem;
}

label {
  display: block;
  margin-bottom: 0.5rem;
  color: #333;
  font-weight: 500;
  font-size: 0.95rem;
}

input {
  width: 100%;
  padding: 0.75rem;
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

.error-message {
  color: #e74c3c;
  font-size: 0.9rem;
  margin-bottom: 1rem;
  text-align: center;
}

.login-btn {
  width: 100%;
  padding: 0.75rem;
  background-color: darkblue;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
}

.login-btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 20px rgba(102, 126, 234, 0.4);
}

.demo-hint {
  text-align: center;
  color: #999;
  font-size: 0.85rem;
  margin-top: 1.5rem;
}
</style>
