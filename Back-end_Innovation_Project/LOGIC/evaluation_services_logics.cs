using Microsoft.EntityFrameworkCore;
using Back_end_Innovation_Project.APP.DTOs;
using Back_end_Innovation_Project.LOGIC.Interfaces;
using Back_end_Innovation_Project.PERSIST;
using Back_end_Innovation_Project.LOGIC.Calculators;
using Back_end_Innovation_Project.MODEL;


namespace Back_end_Innovation_Project.LOGIC.Services;

public class EvaluationServices : IEvaluationService
{
    private readonly AppDb _context; 
    public EvaluationServices(AppDb context)
    {
        _context = context;
    }
    
    public async Task<EvaluationResultDto> EvaluateProjectAsync(EvaluationRequestDto request, string userId)
    {
        //  (STUB) :   ICI  on devras faire l'appel à l'API (OpenAI/Claude)
    var calculator = new ImpactCalculator();
    var result = calculator.EvaluateProject(request);

    var history = new EvaluationHistory
    {
        UserId = userId,
        ModelName = request.AiModel, // Changé de ModelName vers AiModel (pour correspondre au DTO)
        AiScore = "", 
        CarbonFootprint = result.TotalCarbonKg,
        WaterFootprintLiters = result.TotalWaterLiters,
        EnergyKwh = result.TotalEnergyKwh,
        CostUsd = result.TotalCostUsd,
        ValueSavedEur = result.ValueSavedEur,
        
        RecommendedEnvModel = result.RecommendedEnvModel,
        RecommendedEnvComplexity = result.RecommendedEnvComplexity,
        RecommendedEnvEnergyKwh = result.RecommendedEnvEnergyKwh,
        RecommendedEnvWaterLiters = result.RecommendedEnvWaterLiters,
        RecommendedEnvCostUsd = result.RecommendedEnvCostUsd, 

        RecommendedEcoModel = result.RecommendedEcoModel,
        RecommendedEcoComplexity = result.RecommendedEcoComplexity,
        RecommendedEcoEnergyKwh = result.RecommendedEcoEnergyKwh, 
        RecommendedEcoWaterLiters = result.RecommendedEcoWaterLiters, 
        RecommendedEcoCostUsd = result.RecommendedEcoCostUsd,


        RecommendedQualityModel = result.RecommendedQualityModel,
        RecommendedQualityComplexity = result.RecommendedQualityComplexity,
        RecommendedQualityEnergyKwh = result.RecommendedQualityEnergyKwh,
        RecommendedQualityWaterLiters = result.RecommendedQualityWaterLiters,
        RecommendedQualityCostUsd = result.RecommendedQualityCostUsd,
                
        EfficiencyRating = result.EfficiencyRating,
        EnvironmentalRating = result.EnvironmentalRating,
        EconomicRating = result.EconomicRating,
        RiskRating = result.RiskRating,
        VerdictLevel = result.VerdictLevel,

        IsApproved = result.VerdictLevel != "Déconseillé",
        CreatedAt = DateTime.UtcNow
    };

    _context.EvaluationHistory.Add(history);
    await _context.SaveChangesAsync();

    result.EvaluationId = history.Id;

    return result;
    }

    public async Task<(bool Success, string Message)> EvaluateAiScoreAsync(int evaluationId, EvaluationAiScoreDto request, string userId)
    {
        // On récupère l'évaluation depuis la base de données
        var evaluation = await _context.EvaluationHistory
                                       .FirstOrDefaultAsync(e => e.Id == evaluationId && e.UserId == userId);

        if (evaluation == null)
        {
            return (false, "Évaluation introuvable ou vous n'avez pas l'autorisation de la modifier.");
        }

        // Mise à jour de la note
        evaluation.AiScore = request.AiScore;
        await _context.SaveChangesAsync();

        return (true, "La note de l'IA a été enregistrée avec succès.");
    }




    
    public async Task<(bool Success, IEnumerable<EvaluationHistoryDTO> History, IEnumerable<string> Errors)> GetUserHistoryAsync(string userId, double? minCarbon = null, double? maxCarbon = null,string? aiScore = null,  DateTime? startDate = null,DateTime? endDate = null)
    {
        try
        {
            // 1. LA REQUÊTE DE BASE
            var query = _context.EvaluationHistory
                                .Where(h => h.UserId == userId)
                                .AsQueryable();

            // On empile les differents filtre eventuel pour la requete postgre
            if (minCarbon.HasValue)
            {
                query = query.Where(h => h.CarbonFootprint >= minCarbon.Value);
            }

            if (maxCarbon.HasValue)
            {
                query = query.Where(h => h.CarbonFootprint <= maxCarbon.Value);
            }

            if (!string.IsNullOrEmpty(aiScore))
            {
                query = query.Where(h => h.AiScore.ToLower() == aiScore.ToLower());
            }

            if (startDate.HasValue)
            {
                var startUtc = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
                query = query.Where(h => h.CreatedAt >= startUtc);
            }

            if (endDate.HasValue)
            {
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                var endUtc = DateTime.SpecifyKind(endOfDay, DateTimeKind.Utc);
                query = query.Where(h => h.CreatedAt <= endUtc);
            }

            // le tri
            query = query.OrderByDescending(h => h.CreatedAt);

            // On execute
            var evaluations = await query.ToListAsync();

            // mapping du DTO
            var historyDtos = evaluations.Select(e => new EvaluationHistoryDTO
            {
                Id = e.Id.ToString(), 
                ModelName = e.ModelName,
                AiScore = e.AiScore,
                CarbonFootprint = e.CarbonFootprint,
                WaterFootprintLiters = e.WaterFootprintLiters,
                EnergyKwh = e.EnergyKwh,
                CostUsd = e.CostUsd,
                ValueSavedEur = e.ValueSavedEur, // Nouvelle donnée
                
                RecommendedEnvModel = e.RecommendedEnvModel,
                RecommendedEnvComplexity = e.RecommendedEnvComplexity,
                RecommendedEnvEnergyKwh = e.RecommendedEnvEnergyKwh,
                RecommendedEnvWaterLiters = e.RecommendedEnvWaterLiters,
                RecommendedEnvCostUsd = e.RecommendedEnvCostUsd, 

                RecommendedEcoModel = e.RecommendedEcoModel,
                RecommendedEcoComplexity = e.RecommendedEcoComplexity,
                RecommendedEcoEnergyKwh = e.RecommendedEcoEnergyKwh, 
                RecommendedEcoWaterLiters = e.RecommendedEcoWaterLiters, 
                RecommendedEcoCostUsd = e.RecommendedEcoCostUsd,

                RecommendedQualityModel = e.RecommendedQualityModel,
                RecommendedQualityComplexity = e.RecommendedQualityComplexity,
                RecommendedQualityEnergyKwh = e.RecommendedQualityEnergyKwh,
                RecommendedQualityWaterLiters = e.RecommendedQualityWaterLiters,
                RecommendedQualityCostUsd = e.RecommendedQualityCostUsd,
                        
                // Nouvelles notes
                EfficiencyRating = e.EfficiencyRating,
                EnvironmentalRating = e.EnvironmentalRating,
                EconomicRating = e.EconomicRating,
                RiskRating = e.RiskRating,
                VerdictLevel = e.VerdictLevel,
                
                IsApproved = e.IsApproved,
                CreatedAt = e.CreatedAt
            });

            return (true, historyDtos, Array.Empty<string>());
        }
        catch (Exception ex)
        {
            return (false, Enumerable.Empty<EvaluationHistoryDTO>(), new[] { $"Erreur lors de la récupération : {ex.Message}" });
        }
    }
}