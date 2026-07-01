import { reactive } from 'vue'
import axios from 'axios' // À ajouter
import type { Evaluation, SessionTimeline, WorkflowInput } from '@/types'
import { getModel } from '@/data/catalog'
import { generateSessionTimeline } from '@/data/stubs'
import { deriveTokenUsage } from '@/data/timeline'
import { useHistory } from './history'

interface EvaluationState {
  input: WorkflowInput | null
  timeline: SessionTimeline | null
  result: Evaluation | null
}

const state = reactive<EvaluationState>({
  input: null,
  timeline: null,
  result: null,
})

function makeId(): string {
  return `eval-${Date.now().toString(36)}`
}

export function startEvaluation(input: WorkflowInput): SessionTimeline {
  state.input = input
  state.result = null
  state.timeline = generateSessionTimeline(getModel(input.aiModelId))
  return state.timeline
}

// DEVRAIT DEVENIR ASYNC POUR ATTENDRE L'API
export async function finalizeEvaluation(): Promise<Evaluation> {
  if (!state.input || !state.timeline) {
    throw new Error('finalizeEvaluation called before startEvaluation')
  }
  
  // Le simulateur front-end a calculé le nombre de tokens
  const tokens = deriveTokenUsage(state.timeline)

  // 1. Préparation de la requête pour ton DTO C#
  const requestDto = {
    workflowDescription: state.input.workflowDescription,
    runFrequency: state.input.runFrequency,
    employeeCount: state.input.employeeCount,
    hoursPerRun: state.input.hoursPerRun,
    experienceLevel: state.input.experienceLevel,
    aiModel: state.input.aiModelId, // Doit correspondre à "GPT", "Claude", etc.
    complexity: state.input.complexity,
    inputTokens: tokens.inputTokens,
    outputTokens: tokens.outputTokens,
    aiSavingsFraction: state.input.aiSavingsFraction,
    dataSensitivity: state.input.dataSensitivity,
    legalRisk: state.input.legalRisk,
    useCase: state.input.useCase
  }

  try {
    // 2. Appel à ton API C# (remplace le port par le bon)
    // /!\ Le token JWT sera géré dynamiquement plus tard
    const token = localStorage.getItem('jwt_token') || ''
    
    const response = await axios.post('http://localhost:5001/api/Evaluation/calculate', requestDto, {
      headers: { Authorization: `Bearer ${token}` }
    })
    
    const data = response.data

    // 3. Mapping de la réponse dans l'objet Vue
    const evaluation: Evaluation = {
      id: makeId(),
      createdAt: new Date().toISOString(),
      input: state.input,
      timeline: state.timeline,
      
      // Mapping des résultats
      evaluationId: data.evaluationId,
      verdictLevel: data.verdictLevel,
      verdictReason: data.verdictReason,
      gateTriggered: data.gateTriggered,
      efficiencyRating: data.efficiencyRating,
      environmentalRating: data.environmentalRating,
      economicRating: data.economicRating,
      riskRating: data.riskRating,
      totalEnergyKwh: data.totalEnergyKwh,
      totalCarbonKg: data.totalCarbonKg,
      totalWaterLiters: data.totalWaterLiters,
      totalCostUsd: data.totalCostUsd,
      valueSavedEur: data.valueSavedEur,
      
      // Les blocs symétriques
      recommendedEnv: {
        model: data.recommendedEnvModel,
        complexity: data.recommendedEnvComplexity,
        energyKwh: data.recommendedEnvEnergyKwh,
        waterLiters: data.recommendedEnvWaterLiters,
        costUsd: data.recommendedEnvCostUsd
      },
      recommendedEco: {
        model: data.recommendedEcoModel,
        complexity: data.recommendedEcoComplexity,
        energyKwh: data.recommendedEcoEnergyKwh,
        waterLiters: data.recommendedEcoWaterLiters,
        costUsd: data.recommendedEcoCostUsd
      },
      recommendedQuality: {
        model: data.recommendedQualityModel,
        complexity: data.recommendedQualityComplexity,
        energyKwh: data.recommendedQualityEnergyKwh,
        waterLiters: data.recommendedQualityWaterLiters,
        costUsd: data.recommendedQualityCostUsd
      }
    }

    state.result = evaluation
    useHistory().add(evaluation) // Sauvegarde locale (à remplacer par un GET /history plus tard)
    return evaluation
    
  } catch (error) {
    console.error("Erreur lors de l'appel API:", error)
    throw error
  }
}

export function useEvaluation() {
  return {
    state,
    startEvaluation,
    finalizeEvaluation,
  }
}