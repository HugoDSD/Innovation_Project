/**
 * Evaluation history, persisted to localStorage.
 * Each completed evaluation is appended so the user can revisit past verdicts.
 */
import { ref } from 'vue'
import type { Evaluation } from '@/types'

const STORAGE_KEY = 'sobria.history.v1'

function load(): Evaluation[] {
  try {
    const raw = localStorage.getItem(STORAGE_KEY)
    return raw ? (JSON.parse(raw) as Evaluation[]) : []
  } catch {
    return []
  }
}

const history = ref<Evaluation[]>(load())

function persist() {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(history.value))
  } catch {
    // Storage full or unavailable — history is best-effort, ignore.
  }
}

export function useHistory() {
  function add(evaluation: Evaluation) {
    history.value.unshift(evaluation)
    persist()
  }

  function remove(id: string) {
    history.value = history.value.filter((e) => e.id !== id)
    persist()
  }

  function clear() {
    history.value = []
    persist()
  }

  function get(id: string): Evaluation | undefined {
    return history.value.find((e) => e.id === id)
  }

  return { history, add, remove, clear, get }
}
