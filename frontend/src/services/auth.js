import { apiFetch } from './api.js'

export function login(email, password) {
  return apiFetch('/auth/login', {
    method: 'POST',
    body: JSON.stringify({ email, password })
  })
}

export function register(email, password, name, surname, companyName = null) {
  return apiFetch('/auth/register', {
    method: 'POST',
    body: JSON.stringify({ email, password, name, surname, companyName })
  })
}
