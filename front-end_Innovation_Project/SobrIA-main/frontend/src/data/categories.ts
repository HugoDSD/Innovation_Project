/**
 * Visual metadata for the context-window simulator categories.
 * Labels are French (product UI language); keys match `EventCategory`.
 */
import type { EventCategory } from '@/types'

export interface CategoryMeta {
  key: EventCategory
  label: string
  /** Hex used for the stacked gauge segment and legend swatch. */
  color: string
}

export const CATEGORIES: Record<EventCategory, CategoryMeta> = {
  system: { key: 'system', label: 'Système', color: '#64748b' },
  claudeMd: { key: 'claudeMd', label: 'CLAUDE.md', color: '#d97706' },
  memory: { key: 'memory', label: 'Mémoire', color: '#9333ea' },
  skills: { key: 'skills', label: 'Compétences', color: '#0d9488' },
  mcp: { key: 'mcp', label: 'MCP', color: '#2563eb' },
  rules: { key: 'rules', label: 'Règles', color: '#db2777' },
  user: { key: 'user', label: 'Utilisateur', color: '#16a34a' },
  files: { key: 'files', label: 'Fichiers', color: '#ca8a04' },
  output: { key: 'output', label: 'Sortie', color: '#0891b2' },
  claude: { key: 'claude', label: 'Claude', color: '#4f46e5' },
  hooks: { key: 'hooks', label: 'Hooks', color: '#e11d48' },
}

/** Stable display order for the legend. */
export const CATEGORY_ORDER: EventCategory[] = [
  'system',
  'claudeMd',
  'memory',
  'skills',
  'mcp',
  'rules',
  'user',
  'files',
  'output',
  'claude',
  'hooks',
]
