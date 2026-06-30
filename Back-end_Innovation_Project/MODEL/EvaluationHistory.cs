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