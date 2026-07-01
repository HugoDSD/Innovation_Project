/**
 * Static catalog of AI models and cloud providers.
 *
 * Phase 1 stub: figures are plausible placeholders, NOT authoritative. In a
 * later phase model metadata (context window, per-token price) comes from the
 * models.dev API and provider factors from a calibrated table — see
 * `docs/PLAN-IMPLEMENTATION.md` › "Sources de données".
 */
import type { AiModel, CloudProvider, ExperienceLevel } from '@/types'

export const AI_MODELS: AiModel[] = [
  {
    id: 'claude-opus-4',
    name: 'Claude Opus 4',
    vendor: 'Anthropic',
    contextWindow: 200_000,
    priceIn: 15 / 1_000_000,
    priceOut: 75 / 1_000_000,
  },
  {
    id: 'claude-sonnet-4',
    name: 'Claude Sonnet 4',
    vendor: 'Anthropic',
    contextWindow: 200_000,
    priceIn: 3 / 1_000_000,
    priceOut: 15 / 1_000_000,
  },
  {
    id: 'claude-haiku-4',
    name: 'Claude Haiku 4',
    vendor: 'Anthropic',
    contextWindow: 200_000,
    priceIn: 1 / 1_000_000,
    priceOut: 5 / 1_000_000,
  },
  {
    id: 'gpt-5',
    name: 'GPT-5',
    vendor: 'OpenAI',
    contextWindow: 400_000,
    priceIn: 10 / 1_000_000,
    priceOut: 30 / 1_000_000,
  },
  {
    id: 'gemini-2-5-pro',
    name: 'Gemini 2.5 Pro',
    vendor: 'Google',
    contextWindow: 1_000_000,
    priceIn: 7 / 1_000_000,
    priceOut: 21 / 1_000_000,
  },
  {
    id: 'mistral-large-2',
    name: 'Mistral Large 2',
    vendor: 'Mistral',
    contextWindow: 128_000,
    priceIn: 2 / 1_000_000,
    priceOut: 6 / 1_000_000,
  },
]

export const CLOUD_PROVIDERS: CloudProvider[] = [
  // Carbon intensity roughly reflects each region's electricity mix.
  {
    id: 'scaleway-fr',
    name: 'Scaleway',
    region: 'Paris (FR)',
    carbonIntensity: 56,
    waterFactor: 1.8,
    whPerKToken: 0.3,
  },
  {
    id: 'ovh-fr',
    name: 'OVHcloud',
    region: 'Gravelines (FR)',
    carbonIntensity: 52,
    waterFactor: 1.2,
    whPerKToken: 0.3,
  },
  {
    id: 'aws-eu-west-3',
    name: 'AWS',
    region: 'Paris (eu-west-3)',
    carbonIntensity: 58,
    waterFactor: 2.5,
    whPerKToken: 0.35,
  },
  {
    id: 'azure-france-central',
    name: 'Azure',
    region: 'France Central',
    carbonIntensity: 60,
    waterFactor: 2.2,
    whPerKToken: 0.34,
  },
  {
    id: 'gcp-europe-west9',
    name: 'Google Cloud',
    region: 'Paris (europe-west9)',
    carbonIntensity: 62,
    waterFactor: 2.0,
    whPerKToken: 0.33,
  },
  {
    id: 'aws-us-east-1',
    name: 'AWS',
    region: 'Virginie (us-east-1)',
    carbonIntensity: 380,
    waterFactor: 3.6,
    whPerKToken: 0.38,
  },
]

export function getModel(id: string): AiModel {
  return AI_MODELS.find((m) => m.id === id) ?? AI_MODELS[0]!
}

export function getProvider(id: string): CloudProvider {
  return CLOUD_PROVIDERS.find((p) => p.id === id) ?? CLOUD_PROVIDERS[0]!
}

/**
 * Hourly labour cost by experience level (French market, all-in staffing cost).
 * Used to derive valueSaved without asking the user for a rate directly.
 */
export const HOURLY_RATE_BY_LEVEL: Record<ExperienceLevel, number> = {
  junior: 30,
  confirmé: 50,
  senior: 75,
  expert: 110,
}

export const EXPERIENCE_LEVELS: { value: ExperienceLevel; label: string; hint: string }[] = [
  { value: 'junior', label: 'Junior', hint: '~€30/h' },
  { value: 'confirmé', label: 'Confirmé', hint: '~€50/h' },
  { value: 'senior', label: 'Senior', hint: '~€75/h' },
  { value: 'expert', label: 'Expert / Lead', hint: '~€110/h' },
]
