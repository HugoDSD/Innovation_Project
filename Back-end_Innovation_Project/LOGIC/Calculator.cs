using System;
using System.Linq;
using System.Collections.Generic;
using Back_end_Innovation_Project.APP.DTOs;

namespace Back_end_Innovation_Project.LOGIC.Calculators;

public class ImpactCalculator
{
    // Seuils extraits de ratings.ts
    private readonly double[] EfficiencyThresholds = { 200, 800, 2500, 8000 };
    private readonly double[] Co2Thresholds = { 0.5, 2, 8, 25 };
    private readonly double[] WaterThresholds = { 15, 60, 250, 800 };
    private readonly double[] CostRatioThresholds = { 0.01, 0.05, 0.20, 0.50 };

    private readonly Dictionary<string, double> HourlyRates = new(StringComparer.OrdinalIgnoreCase)
    {
        { "junior", 30 }, { "confirmé", 50 }, { "senior", 75 }, { "expert", 110 }
    };

    private readonly List<string> SensitivityOrder = new() { "public", "interne", "confidentiel", "réglementé" };
    private readonly List<string> LegalOrder = new() { "faible", "modéré", "élevé", "critique" };
    private readonly int[] RiskDominanceTable = { 5, 4, 2, 1 };

    // Constantes physiques
    private const double MixElectriqueFrance = 0.0801;
    
    private readonly Dictionary<string, Dictionary<string, ModelSpecs>> _aiSpecs = new(StringComparer.OrdinalIgnoreCase)
    {
        { "GPT", new(StringComparer.OrdinalIgnoreCase) {
            { "petit", new ModelSpecs(1.5e-07, 6e-07, 7.88888888888889e-09, 0.03) },
            { "grand", new ModelSpecs(3e-06, 1.5e-05, 1.1027777777777778e-08, 0.03) }
        }},
        { "Claude", new(StringComparer.OrdinalIgnoreCase) {
            { "petit", new ModelSpecs(3e-06, 1.5e-05, 7.88888888888889e-09, 0.12) },
            { "grand", new ModelSpecs(5e-06, 2.5e-05, 1.1027777777777778e-08, 0.12) }
        }},
        { "DeepSeek", new(StringComparer.OrdinalIgnoreCase) {
            { "petit", new ModelSpecs(1.4e-07, 2.8e-07, 3.708611111111111e-07, 0.84) },
            { "grand", new ModelSpecs(4.35e-07, 8.7e-07, 6.600555555555555e-07, 0.84) }
        }}
    };

    private readonly Dictionary<string, string> _useCaseExperts = new(StringComparer.OrdinalIgnoreCase)
    {
        // Spécialités GPT
        { "rédaction business", "GPT" },
        { "code du quotidien (requête SQL...etc)", "GPT" },
        { "assistant quotidien", "GPT" },
        
        // Spécialités Claude
        { "code dev", "Claude" },
        { "analyse de document", "Claude" },
        { "rédaction rapport", "Claude" },
        
        // Spécialités DeepSeek
        { "décisions logique", "DeepSeek" },
        { "code technique (debug, algorithme)", "DeepSeek" },
        { "raisonnement dans un probleme complexe", "DeepSeek" }
    };


    public EvaluationResultDto EvaluateProject(EvaluationRequestDto request)
    {
        var result = new EvaluationResultDto();


        if (!_aiSpecs.TryGetValue(request.AiModel, out var complexities) || 
            !complexities.TryGetValue(request.Complexity, out var specs))
        {
            throw new ArgumentException($"Modèle ou complexité invalide : {request.AiModel} - {request.Complexity}");
        }
        // Calculs physiques 
        double totalTokens = request.InputTokens + request.OutputTokens;
        
        result.TotalEnergyKwh = totalTokens * specs.EnergyPerToken * request.RunFrequency;
        result.TotalCarbonKg = result.TotalEnergyKwh * MixElectriqueFrance;
        result.TotalWaterLiters = result.TotalEnergyKwh * specs.WaterPerKwh;
        result.TotalCostUsd = ((request.InputTokens * specs.InputCost) + (request.OutputTokens * specs.OutputCost)) * request.RunFrequency;

        double rate = HourlyRates.GetValueOrDefault(request.ExperienceLevel, 50);
        double hoursSavedPerRun = request.AiSavingsFraction * request.EmployeeCount * request.HoursPerRun;
        result.ValueSavedEur = hoursSavedPerRun * rate * request.RunFrequency;

        // On note de 1 à 5
        result.EfficiencyRating = RateHigherBetter(result.ValueSavedEur, EfficiencyThresholds);
        
        int co2Rating = RateLowerBetter(result.TotalCarbonKg, Co2Thresholds);
        int waterRating = RateLowerBetter(result.TotalWaterLiters, WaterThresholds);
        result.EnvironmentalRating = (int)Math.Round(0.5 * co2Rating + 0.5 * waterRating);

        double costRatio = result.ValueSavedEur > 0 ? result.TotalCostUsd / result.ValueSavedEur : double.PositiveInfinity;
        result.EconomicRating = RateLowerBetter(costRatio, CostRatioThresholds);

        int sIdx = Math.Max(0, SensitivityOrder.IndexOf(request.DataSensitivity.ToLower()));
        int lIdx = Math.Max(0, LegalOrder.IndexOf(request.LegalRisk.ToLower()));
        int worstRisk = Math.Max(sIdx, lIdx);
        result.RiskRating = RiskDominanceTable[Math.Min(worstRisk, 3)];

        //  Application du Verdict
        ComputeVerdict(result);

        

        FindBestAlternative(request, totalTokens, result);

        string qualityModel = _useCaseExperts.GetValueOrDefault(request.UseCase, request.AiModel);
        string qualityComplexity = request.Complexity; // On garde la taille demandée par l'utilisateur

        // On va chercher les specs de ce modèle expert dans notre dictionnaire
        if (_aiSpecs.TryGetValue(qualityModel, out var qComplexities) &&
            qComplexities.TryGetValue(qualityComplexity, out var qSpecs))
        {
            result.RecommendedQualityModel = qualityModel;
            result.RecommendedQualityComplexity = qualityComplexity;
            result.RecommendedQualityEnergyKwh = totalTokens * qSpecs.EnergyPerToken * request.RunFrequency;
            result.RecommendedQualityWaterLiters = result.RecommendedQualityEnergyKwh * qSpecs.WaterPerKwh;
            result.RecommendedQualityCostUsd = ((request.InputTokens * qSpecs.InputCost) + (request.OutputTokens * qSpecs.OutputCost)) * request.RunFrequency;
        }
        else
        {
            // Sécurité : si on ne trouve pas (ne devrait jamais arriver), on remet le modèle par défaut
            result.RecommendedQualityModel = request.AiModel;
            result.RecommendedQualityComplexity = request.Complexity;
            result.RecommendedQualityEnergyKwh = result.TotalEnergyKwh;
            result.RecommendedQualityWaterLiters = result.TotalWaterLiters;
            result.RecommendedQualityCostUsd = result.TotalCostUsd;
        }

        return result;
    }

    private int RateHigherBetter(double value, double[] t)
    {
        if (value >= t[3]) return 5;
        if (value >= t[2]) return 4;
        if (value >= t[1]) return 3;
        if (value >= t[0]) return 2;
        return 1;
    }

    private int RateLowerBetter(double value, double[] t)
    {
        if (value <= t[0]) return 5;
        if (value <= t[1]) return 4;
        if (value <= t[2]) return 3;
        if (value <= t[3]) return 2;
        return 1;
    }

    private void ComputeVerdict(EvaluationResultDto r)
    {
        int gateThreshold = 2;

        if (r.RiskRating <= gateThreshold)
        {
            r.VerdictLevel = "Déconseillé";
            r.GateTriggered = "risk-veto";
            r.VerdictReason = $"Risque trop élevé (Risque {r.RiskRating}/5) — un risque élevé n'est pas compensable par du temps gagné.";
            return;
        }

        if (r.EfficiencyRating <= gateThreshold)
        {
            r.VerdictLevel = "Déconseillé";
            r.GateTriggered = "efficiency-floor";
            r.VerdictReason = $"Gain de temps négligeable (Efficacité {r.EfficiencyRating}/5) — sans bénéfice, rien à arbitrer.";
            return;
        }

        if (r.EnvironmentalRating <= gateThreshold && r.EconomicRating <= gateThreshold)
        {
            r.VerdictLevel = "Déconseillé";
            r.GateTriggered = "double-cost";
            r.VerdictReason = $"Empreinte environnementale et coût tous deux trop élevés (Environnemental {r.EnvironmentalRating}/5, Économique {r.EconomicRating}/5) — optimiser ne suffit plus, reconsidérer l'usage.";
            return;
        }

        if (r.EnvironmentalRating <= gateThreshold || r.EconomicRating <= gateThreshold)
        {
            r.VerdictLevel = "À optimiser";
            r.GateTriggered = "cost-or-footprint";
            string culprit = r.EnvironmentalRating <= gateThreshold 
                ? $"empreinte environnementale élevée (Environnemental {r.EnvironmentalRating}/5)"
                : $"coût élevé (Économique {r.EconomicRating}/5)";
            r.VerdictReason = $"Usage utile et sûr mais {culprit} — à employer avec sobriété.";
            return;
        }

        r.VerdictLevel = "Recommandé";
        r.GateTriggered = "pass";
        r.VerdictReason = "La valeur justifie l'impact sur les quatre critères — bon choix pour ce workflow.";
    }







    private void FindBestAlternative(EvaluationRequestDto request, double totalTokens, EvaluationResultDto result)
    {
        string bestEnvModel = request.AiModel;
        string bestEnvComplexity = request.Complexity;
        double minEnvImpact = double.MaxValue;
        double bestEnvEnergy = result.TotalEnergyKwh;
        double bestEnvWater = result.TotalWaterLiters;
        double bestEnvCost = result.TotalCostUsd;


        string bestEcoModel = request.AiModel;
        string bestEcoComplexity = request.Complexity;
        double minCost = double.MaxValue;
        double bestEcoCost = result.TotalCostUsd;
        double bestEcoEnergy = result.TotalEnergyKwh; 
        double bestEcoWater = result.TotalWaterLiters;
        // On parcours tous les modèles et complexités pour trouver le plus écologique et le plus economique
        foreach (var modelKvp in _aiSpecs)
        {
            foreach (var compKvp in modelKvp.Value)
            {
                var tempSpecs = compKvp.Value;
                
                double tempEnergy = totalTokens * tempSpecs.EnergyPerToken * request.RunFrequency;
                double tempWater = tempEnergy * tempSpecs.WaterPerKwh;
                double tempCarbon = tempEnergy * MixElectriqueFrance;
                double tempCost = ((request.InputTokens * tempSpecs.InputCost) + (request.OutputTokens * tempSpecs.OutputCost)) * request.RunFrequency;

                // Évaluation Environnementale (avec un score combiné et un taux de conv de l'eau a l'aide d'un ratio de 0.01)
                double impactScore = tempCarbon + (tempWater * 0.01); 
                if (impactScore < minEnvImpact)
                {
                    minEnvImpact = impactScore;
                    bestEnvModel = modelKvp.Key;
                    bestEnvComplexity = compKvp.Key;
                    bestEnvEnergy = tempEnergy;
                    bestEnvWater = tempWater;
                    bestEnvCost = tempCost; 
                }

                //  Évaluation Économique 
                if (tempCost < minCost)
                {
                    minCost = tempCost;
                    bestEcoModel = modelKvp.Key;
                    bestEcoComplexity = compKvp.Key;
                    bestEcoCost = tempCost;
                    bestEcoEnergy = tempEnergy; 
                    bestEcoWater = tempWater; 
                }
            }
        }

        result.RecommendedEnvModel = bestEnvModel;
        result.RecommendedEnvComplexity = bestEnvComplexity;
        result.RecommendedEnvEnergyKwh = bestEnvEnergy;
        result.RecommendedEnvWaterLiters = bestEnvWater;
        result.RecommendedEnvCostUsd = bestEnvCost; 

        // Affectation des résultats Économiques
        result.RecommendedEcoModel = bestEcoModel;
        result.RecommendedEcoComplexity = bestEcoComplexity;
        result.RecommendedEcoEnergyKwh = bestEcoEnergy; 
        result.RecommendedEcoWaterLiters = bestEcoWater; 
        result.RecommendedEcoCostUsd = bestEcoCost;
        }
}
    public record ModelSpecs(double InputCost, double OutputCost, double EnergyPerToken, double WaterPerKwh);
