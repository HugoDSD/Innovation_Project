using System;
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
    
    private readonly Dictionary<string, double> _energyPerToken = new() {
        { "GPT OSS 20B", 7.888889e-09 }, { "GPT OSS 120B", 1.102778e-08 },
        { "DeepSeek V3.1", 3.708611e-07 }, { "DeepSeek R1", 6.600556e-07 }
    };

    private readonly Dictionary<string, double> _wueProvider = new() {
        { "Microsoft", 0.03 }, { "Amazon", 0.12 }, { "Référence", 0.84 }
    };

    public EvaluationResultDto EvaluateProject(EvaluationRequestDto request, double inputCost, double outputCost)
    {
        var result = new EvaluationResultDto();

        // 1. Calculs physiques déterministes
        double energyPerToken = _energyPerToken.GetValueOrDefault(request.AiModel, 1.0e-08);
        double totalTokens = request.InputTokens + request.OutputTokens;
        
        result.TotalEnergyKwh = totalTokens * energyPerToken * request.RunFrequency;
        result.TotalCarbonKg = result.TotalEnergyKwh * MixElectriqueFrance;
        result.TotalWaterLiters = result.TotalEnergyKwh * _wueProvider.GetValueOrDefault(request.CloudProvider, 0.84);

        //  Utilisation des coûts injectés dynamiquement depuis l'api
        result.TotalCostUsd = ((request.InputTokens * inputCost) + (request.OutputTokens * outputCost)) * request.RunFrequency;

        double rate = HourlyRates.GetValueOrDefault(request.ExperienceLevel, 50);
        double hoursSavedPerRun = request.AiSavingsFraction * request.EmployeeCount * request.HoursPerRun;
        result.ValueSavedEur = hoursSavedPerRun * rate * request.RunFrequency;

        // 2. Notation de 1 à 5
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

        // 3. Application du Verdict
        ComputeVerdict(result);

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
}