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


        public double CarbonFootprint { get; set; }
        public double WaterFootprintLiters { get; set; }
        public double EnergyKwh { get; set; }

        public double CostUsd { get; set; }
        public double HoursSaved { get; set; }
        public double RiskScore { get; set; }


        public bool IsApproved { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}