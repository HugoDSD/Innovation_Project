// src/services/apiService.ts
import type { AuthResponse, RegisterRequest, LoginRequest, EvaluationFilters } from '../types/api.ts';

const API_BASE_URL = 'http://localhost:5051/api';

class ApiService {
  private token: string | null;

  constructor() {
    this.token = localStorage.getItem('token');
  }

  setToken(token: string): void {
    this.token = token;
    localStorage.setItem('token', token);
  }

  private getHeaders(): HeadersInit {
    const headers: Record<string, string> = {
      'Content-Type': 'application/json'
    };
    if (this.token) {
      headers['Authorization'] = `Bearer ${this.token}`;
    }
    return headers;
  }

  async register(data: RegisterRequest): Promise<any> {
    const response = await fetch(`${API_BASE_URL}/Auth/register`, {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify(data)
    });
    if (!response.ok) throw new Error('Inscription échouée');
    return response.json();
  }

  async login(email: string, password: string) {
    const response = await fetch(`${API_BASE_URL}/Auth/login`, {
        method: 'POST',
        headers: this.getHeaders(),
        body: JSON.stringify({
            email: email,       
            password: password  
        })
    });
  }

  async calculateEvaluation(evaluationData: any): Promise<any> {
    const response = await fetch(`${API_BASE_URL}/Evaluation/calculate`, {
      method: 'POST',
      headers: this.getHeaders(),
      body: JSON.stringify(evaluationData)
    });
    if (!response.ok) throw new Error('Erreur lors du calcul');
    return response.json();
  }

  async updateEvaluationScore(evaluationId: number, aiScore: string): Promise<any> {
    const response = await fetch(`${API_BASE_URL}/Evaluation/${evaluationId}/score`, {
      method: 'PUT',
      headers: this.getHeaders(),
      body: JSON.stringify({ aiScore })
    });
    if (!response.ok) throw new Error('Erreur lors de la mise à jour');
    return response.json();
  }

  async getEvaluationHistory(filters: EvaluationFilters = {}): Promise<any[]> {
    const params = new URLSearchParams();
    // On convertit proprement les filtres en string pour l'URL
    (Object.keys(filters) as Array<keyof EvaluationFilters>).forEach(key => {
      if (filters[key] !== undefined) params.append(key, String(filters[key]));
    });

    const response = await fetch(`${API_BASE_URL}/Evaluation/history?${params.toString()}`, {
      method: 'GET',
      headers: this.getHeaders()
    });
    if (!response.ok) throw new Error('Erreur lors de la récupération de l\'historique');
    return response.json();
  }

  logout(): void {
    this.token = null;
    localStorage.removeItem('token');
  }
}

export default new ApiService();