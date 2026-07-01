/** Shared formatting helpers (French locale). */

const eur = new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'EUR', maximumFractionDigits: 0 })
const usd = new Intl.NumberFormat('fr-FR', { style: 'currency', currency: 'USD', maximumFractionDigits: 2 })
const int = new Intl.NumberFormat('fr-FR')

export const formatEur = (n: number) => eur.format(n)
export const formatUsd = (n: number) => usd.format(n)
export const formatInt = (n: number) => int.format(Math.round(n))

export function formatTokens(n: number): string {
  if (n >= 1000) return `${(n / 1000).toFixed(n >= 10_000 ? 0 : 1)} k`
  return int.format(n)
}

export function formatNumber(n: number, digits = 2): string {
  return new Intl.NumberFormat('fr-FR', { maximumFractionDigits: digits }).format(n)
}

export function formatDate(iso: string): string {
  return new Date(iso).toLocaleString('fr-FR', {
    dateStyle: 'medium',
    timeStyle: 'short',
  })
}
