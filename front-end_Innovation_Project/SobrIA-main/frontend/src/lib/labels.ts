/** French display labels and styling for criteria and verdicts. */
import type { CriterionKey, VerdictLevel } from '@/types'

export const CRITERION_LABELS: Record<CriterionKey, string> = {
  efficiency: 'Efficacité',
  environmental: 'Environnemental',
  economic: 'Économique',
  risk: 'Risque',
}

export const CRITERION_ORDER: CriterionKey[] = ['efficiency', 'environmental', 'economic', 'risk']

export const CRITERION_KIND: Record<CriterionKey, 'bénéfice' | 'coût'> = {
  efficiency: 'bénéfice',
  environmental: 'coût',
  economic: 'coût',
  risk: 'coût',
}

export interface VerdictStyle {
  /** CSS color token name. */
  color: string
  bg: string
  border: string
  icon: string
  blurb: string
}

export const VERDICT_STYLES: Record<VerdictLevel, VerdictStyle> = {
  Recommandé: {
    color: 'var(--color-verdict-good)',
    bg: 'rgba(47, 143, 91, 0.10)',
    border: 'rgba(47, 143, 91, 0.35)',
    icon: '✓',
    blurb: 'La valeur justifie l\'impact — l\'IA est un bon choix pour ce workflow.',
  },
  'À optimiser': {
    color: 'var(--color-verdict-warn)',
    bg: 'rgba(201, 138, 20, 0.10)',
    border: 'rgba(201, 138, 20, 0.35)',
    icon: '◐',
    blurb: 'L\'usage vaut le coup, mais son empreinte est trop élevée — à employer avec sobriété.',
  },
  Déconseillé: {
    color: 'var(--color-verdict-bad)',
    bg: 'rgba(192, 70, 58, 0.10)',
    border: 'rgba(192, 70, 58, 0.35)',
    icon: '✕',
    blurb: 'L\'impact dépasse la valeur apportée, ou le risque est trop élevé — mieux vaut s\'en passer.',
  },
}
