// Service API pour communiquer avec le backend
const API_BASE_URL = 'http://localhost:5051/api'

class ApiService {
  constructor() {
    this.token = localStorage.getItem('token')
  }

  setToken(token) {
    this.token = token
    localStorage.setItem('token', token)
  }

  getHeaders() {
    const headers = {
      'Content-Type': 'application/json'
    }
    if (this.token) {
      headers['Authorization'] = `Bearer ${this.token}`
    }
    return headers
  }

  async register(email, password, name, surname, companyName) {
    try {
      const response = await fetch(`${API_BASE_URL}/Auth/register`, {
        method: 'POST',
        headers: this.getHeaders(),
        body: JSON.stringify({
          Email: email,        // Majuscule ici
          Password: password,  // Majuscule ici
          Name: name,          // Majuscule ici
          Surname: surname,    // Majuscule ici
          CompanyName: companyName // Majuscule ici
        })
      })
      
      if (!response.ok) {
        // Optionnel : affiche l'erreur détaillée si le backend la renvoie
        const errorData = await response.json();
        throw new Error(errorData.message || 'Inscription échouée');
      }
      
      return await response.json();
    } catch (error) {
      throw error
    }
  }

  async login(email, password) {
    try {
      const response = await fetch(`${API_BASE_URL}/Auth/login`, {
        method: 'POST',
        headers: this.getHeaders(),
        body: JSON.stringify({
          email,
          password
        })
      })
      
      if (!response.ok) {
        throw new Error('Connexion échouée')
      }
      
      const data = await response.json()
      this.setToken(data.token)
      return data
    } catch (error) {
      throw error
    }
  }

  async calculateEvaluation(evaluationData) {
    try {
      console.log("Headers envoyés :", this.getHeaders());
      console.log("Données envoyées :", evaluationData);
      const response = await fetch(`${API_BASE_URL}/Evaluation/calculate`, {
        method: 'POST',
        headers: this.getHeaders(),
        body: JSON.stringify(evaluationData)
      })
      
      if (!response.ok) {
        throw new Error('Erreur lors du calcul')
      }
      
      const data = await response.json()
      return data
    } catch (error) {
      throw error
    }
  }

  async updateEvaluationScore(evaluationId, aiScore) {
    try {
      const response = await fetch(`${API_BASE_URL}/Evaluation/${evaluationId}/score`, {
        method: 'PUT',
        headers: this.getHeaders(),
        body: JSON.stringify({
          aiScore
        })
      })
      
      if (!response.ok) {
        throw new Error('Erreur lors de la mise à jour')
      }
      
      const data = await response.json()
      return data
    } catch (error) {
      throw error
    }
  }

  async getEvaluationHistory(filters = {}) {
    try {
      const params = new URLSearchParams()
      if (filters.minCarbon !== undefined) params.append('minCarbon', filters.minCarbon)
      if (filters.maxCarbon !== undefined) params.append('maxCarbon', filters.maxCarbon)
      if (filters.aiScore) params.append('aiScore', filters.aiScore)
      if (filters.startDate) params.append('startDate', filters.startDate)
      if (filters.endDate) params.append('endDate', filters.endDate)

      const url = `${API_BASE_URL}/Evaluation/history?${params.toString()}`
      
      const response = await fetch(url, {
        method: 'GET',
        headers: this.getHeaders()
      })
      
      if (!response.ok) {
        throw new Error('Erreur lors de la récupération de l\'historique')
      }
      
      const data = await response.json()
      return data
    } catch (error) {
      throw error
    }
  }

  logout() {
    this.token = null
    localStorage.removeItem('token')
  }
}

export default new ApiService()
