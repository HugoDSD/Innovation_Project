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
        //On utilise le calculateur pour obtenir les métriques et la décision d'approbation
        var calculator = new ImpactCalculator();
        var result = calculator.EvaluateProject(request);

        if (!result.IsApproved && result.Message.StartsWith("ERREUR"))
        {
            return result;
        }

        // 3. On prépare la "boîte" pour PostgreSQL avec toutes les nouvelles métriques
        var history = new EvaluationHistory
        {
            UserId = userId,
            ModelName = request.ModelName,
            AiScore = "", // La note de l'IA est vide au moment du calcul et sera remplie plus tard par l'utilisateur par EvaluateAiScore
            CarbonFootprint = result.TotalCarbonKg,
            WaterFootprintLiters = result.TotalWaterLiters,
            EnergyKwh = result.TotalEnergyKwh,
            CostUsd = result.TotalCostUsd,
            HoursSaved = result.TotalHoursSaved,
            RiskScore = result.RiskScore,
            IsApproved = result.IsApproved,
            CreatedAt = DateTime.UtcNow
        };

        // On sauvegarde dans la base de données 
        _context.EvaluationHistory.Add(history);
        await _context.SaveChangesAsync();


        result.EvaluationId = history.Id;


        return result;
    }

    public async Task<EvaluationResultDto> EvaluateAiScoreAsync(int evaluationId, EvaluationAiScoreDto request, string userId)
    {
        // On récupère l'évaluation depuis la base de données
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
            // 1. LA REQUÊTE DE BASE (On cible l'utilisateur connecté)
            // AsQueryable() permet de préparer la requête sans l'exécuter tout de suite.
            var query = _context.EvaluationHistory
                                .Where(h => h.UserId == userId)
                                .AsQueryable();

            // 2. L'EMPILAGE DES FILTRES (Dynamique)
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
                // Comparaison insensible à la casse au cas où le front enverrait "utile" au lieu de "Utile"
                query = query.Where(h => h.AiScore.ToLower() == aiScore.ToLower());
            }

            if (startDate.HasValue)
            {
                var startUtc = DateTime.SpecifyKind(startDate.Value, DateTimeKind.Utc);
                query = query.Where(h => h.CreatedAt >= startUtc);
            }

            if (endDate.HasValue)
            {
                // On met l'heure à 23:59:59 pour inclure toute la journée de fin
                var endOfDay = endDate.Value.Date.AddDays(1).AddTicks(-1);
                var endUtc = DateTime.SpecifyKind(endOfDay, DateTimeKind.Utc);
                query = query.Where(h => h.CreatedAt <= endUtc);
            }

            // 3. LE TRI (Optionnel mais recommandé)
            // On trie du plus récent au plus ancien, c'est plus logique pour un historique.
            query = query.OrderByDescending(h => h.CreatedAt);

            // 4. L'EXÉCUTION (Le moment où on envoie le SQL généré à PostgreSQL)
            var evaluations = await query.ToListAsync();

            // 5. LE MAPPING VERS LE DTO (Boîte de sortie)
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
            // En cas de problème de connexion à la base de données
            return (false, Enumerable.Empty<EvaluationHistoryDTO>(), new[] { $"Erreur lors de la récupération : {ex.Message}" });
        }
    }
}