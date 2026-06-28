using Microsoft.EntityFrameworkCore;
using InnovationProject.App.DTOs;
using InnovationProject.Logic.Interfaces;
using InnovationProject.Persist;
using InnovationProject.Logic.Calculators;
using InnovationProject.Model;


namespace InnovationProject.Logic.Services;

public class EvaluationServices : IEvaluationService
{
    private readonly AppDb _context; 
    public EvaluationServices(AppDb context)
    {
        _context = context;
    }
    
    public async Task<EvaluationResultDto> EvaluateProjectAsync(EvaluationRequestDto request, string userId)
    {
        // Use the calculator to get the metrics and the approval decision
        var calculator = new ImpactCalculator();
        var result = calculator.EvaluateProject(request);

        if (!result.IsApproved && result.Message.StartsWith("ERREUR"))
        {
            return result;
        }

        // 3. Prepare the record for PostgreSQL with all the new metrics
        var history = new EvaluationHistory
        {
            UserId = userId,
            ModelName = request.ModelName,
            AiScore = "", // The AI score is empty at calculation time and is filled in later by the user via EvaluateAiScore
            CarbonFootprint = result.TotalCarbonKg,
            WaterFootprintLiters = result.TotalWaterLiters,
            EnergyKwh = result.TotalEnergyKwh,
            CostUsd = result.TotalCostUsd,
            HoursSaved = result.TotalHoursSaved,
            RiskScore = result.RiskScore,
            IsApproved = result.IsApproved,
            CreatedAt = DateTime.UtcNow
        };

        // Save to the database
        _context.EvaluationHistory.Add(history);
        await _context.SaveChangesAsync();


        result.EvaluationId = history.Id;


        return result;
    }

    public async Task<EvaluationResultDto> EvaluateAiScoreAsync(int evaluationId, EvaluationAiScoreDto request, string userId)
    {
        // Retrieve the evaluation from the database
        var evaluation = await _context.EvaluationHistory
                                       .FirstOrDefaultAsync(e => e.Id == evaluationId && e.UserId == userId);

        if (evaluation == null)
        {
            return new EvaluationResultDto
            {
                IsApproved = false,
                Message = "Évaluation introuvable ou vous n'avez pas l'autorisation de la modifier."
            };
        }

        evaluation.AiScore = request.AiScore;

        await _context.SaveChangesAsync();

        return new EvaluationResultDto
        {
            IsApproved = true,
            Message = "La note de l'IA a été enregistrée avec succès.",
            EvaluationId = evaluation.Id
        };
    }





    public async Task<(bool Success, IEnumerable<EvaluationHistoryDTO> History, IEnumerable<string> Errors)> GetUserHistoryAsync(string userId, double? minCarbon = null, double? maxCarbon = null,string? aiScore = null,  DateTime? startDate = null,DateTime? endDate = null)
    {
        try
        {
            // 1. BASE QUERY (target the logged-in user)
            // AsQueryable() prepares the query without executing it immediately.
            var query = _context.EvaluationHistory
                                .Where(h => h.UserId == userId)
                                .AsQueryable();

            // 2. STACKING THE FILTERS (dynamic)
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
                // Case-insensitive comparison in case the frontend sends "utile" instead of "Utile"
                query = query.Where(h => h.AiScore.ToLower() == aiScore.ToLower());
            }

            if (startDate.HasValue)
            {
                var startUtc = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
                query = query.Where(h => h.CreatedAt >= startUtc);
            }

            if (endDate.HasValue)
            {
                // Set the time to 23:59:59 to include the whole end day
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                var endUtc = DateTime.SpecifyKind(endOfDay, DateTimeKind.Utc);
                query = query.Where(h => h.CreatedAt <= endUtc);
            }

            // 3. SORTING (optional but recommended)
            // Sort from newest to oldest, which makes more sense for a history.
            query = query.OrderByDescending(h => h.CreatedAt);

            // 4. EXECUTION (when the generated SQL is sent to PostgreSQL)
            var evaluations = await query.ToListAsync();

            // 5. MAPPING TO THE DTO (output object)
            var historyDtos = evaluations.Select(e => new EvaluationHistoryDTO
            {
                Id = e.Id.ToString(), 
                ModelName = e.ModelName,
                AiScore = e.AiScore,
                CarbonFootprint = e.CarbonFootprint,
                WaterFootprintLiters = e.WaterFootprintLiters,
                EnergyKwh = e.EnergyKwh,
                CostUsd = e.CostUsd,
                HoursSaved = e.HoursSaved,
                RiskScore = e.RiskScore,
                IsApproved = e.IsApproved,
                CreatedAt = e.CreatedAt
            });

            return (true, historyDtos, Array.Empty<string>());
        }
        catch (Exception ex)
        {
            // In case of a database connection problem
            return (false, Enumerable.Empty<EvaluationHistoryDTO>(), new[] { $"Erreur lors de la récupération : {ex.Message}" });
        }
    }
}