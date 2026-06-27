import { apiFetch } from './api.js'

export function calculateImpact(payload) {
  return apiFetch('/Evaluation/calculate', {
    method: 'POST',
    body: JSON.stringify(payload)
  })
}

export function setAiScore(evaluationId, aiScore) {
  return apiFetch(`/Evaluation/${evaluationId}/score`, {
    method: 'PUT',
    body: JSON.stringify({ aiScore })
  })
}

export function getHistory(filters = {}) {
  const params = new URLSearchParams()
  if (filters.minCarbon != null) params.append('minCarbon', filters.minCarbon)
  if (filters.maxCarbon != null) params.append('maxCarbon', filters.maxCarbon)
  if (filters.aiScore) params.append('aiScore', filters.aiScore)
  if (filters.startDate) params.append('startDate', filters.startDate)
  if (filters.endDate) params.append('endDate', filters.endDate)
  const query = params.toString()
  return apiFetch(`/Evaluation/history${query ? `?${query}` : ''}`)
}
