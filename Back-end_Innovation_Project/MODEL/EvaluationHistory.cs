using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Back_end_Innovation_Project.COMMON;


namespace Back_end_Innovation_Project.MODEL
{
    public class EvaluationHistory
    {
        [Key] // Cela force Entity Framework à utiliser cette propriété comme Clé Primaire
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Préparation du terrain pour tes futurs calculs environnementaux
        public string? AiScore { get; set; } // Ex: "UTILE", "MOYEN", "MIEUX SANS IA"
        public double? EstimatedCarbonFootprint { get; set; } // En kg CO2e

    // Clé étrangère pour lier cette sauvegarde au bon utilisateur
        public string UserId { get; set; } = string.Empty;
        public required string AppUserId { get; set; }
    
        [ForeignKey("AppUserId")]
        public AppUser? User { get; set; }
    }
}