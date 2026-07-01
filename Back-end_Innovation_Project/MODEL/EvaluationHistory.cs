using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Back_end_Innovation_Project.COMMON;


namespace Back_end_Innovation_Project.MODEL
{
   public class EvaluationHistory
    {
        [Key]
        public int Id { get; set; }

        public required string UserId { get; set; }
        [ForeignKey("UserId")]
        public AppUser? User { get; set; }
  
        public string ModelName { get; set; } = string.Empty;
        public string AiScore { get; set; } = string.Empty;



        // Métriques physiques
        public double CarbonFootprint { get; set; }
        public double WaterFootprintLiters { get; set; }
        public double EnergyKwh { get; set; }
        public double CostUsd { get; set; }
        public double ValueSavedEur { get; set; } 


         // --- RECOMMANDATION ECOLOGIQUE  ---
        public string RecommendedEnvModel { get; set; } = string.Empty;
        public string RecommendedEnvComplexity { get; set; } = string.Empty;
        public double RecommendedEnvEnergyKwh { get; set; }
        public double RecommendedEnvWaterLiters { get; set; }
        public double RecommendedEnvCostUsd { get; set; } 

        // --- RECOMMANDATION ECONOMIQUE  ---
        public string RecommendedEcoModel { get; set; } = string.Empty;
        public string RecommendedEcoComplexity { get; set; } = string.Empty;
        public double RecommendedEcoEnergyKwh { get; set; } 
        public double RecommendedEcoWaterLiters { get; set; } 
        public double RecommendedEcoCostUsd { get; set; }

        // --- RECOMMANDATION USAGE ---
        public string RecommendedQualityModel { get; set; } = string.Empty;
        public string RecommendedQualityComplexity { get; set; } = string.Empty;
        public double RecommendedQualityEnergyKwh { get; set; }
        public double RecommendedQualityWaterLiters { get; set; }
        public double RecommendedQualityCostUsd { get; set; }


        // Nouvelles notes (1 à 5)
        public int EfficiencyRating { get; set; }
        public int EnvironmentalRating { get; set; }
        public int EconomicRating { get; set; }
        public int RiskRating { get; set; }

        // Verdict
        public string VerdictLevel { get; set; } = string.Empty;

        // ⚠️ TEMPORAIRE : On garde ce champ pour ne pas casser immédiatement tes autres requêtes GET, 
        // mais il est techniquement redondant avec VerdictLevel.
        public bool IsApproved { get; set; } 
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}