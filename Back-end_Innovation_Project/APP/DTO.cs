using System.ComponentModel.DataAnnotations;

using System.ComponentModel.DataAnnotations.Schema;
using Back_end_Innovation_Project.MODEL;
namespace Back_end_Innovation_Project.APP.DTOs;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(8)]
    public required string Password { get; set; }

    public required string Name { get; set; }
    public required string Surname { get; set; }
    
    public string? CompanyName { get; set; }
}


public class LoginDto
{
    [Required]
    [EmailAddress]
    public required string Email {get;set;}

    public required string Password {get;set;}

}



public class EvaluationHistoryDTO
{
    public string Id { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public string AiScore { get; set; } = string.Empty; 

    // --- Les métriques physiques du model choisi par l'utilisateur ---
    public double CarbonFootprint { get; set; }
    public double WaterFootprintLiters { get; set; }
    public double EnergyKwh { get; set; }
    public double CostUsd { get; set; }
    public double ValueSavedEur { get; set; } // Remplace "HoursSaved"


    // --- Comparaison d'ia sur l'environement et l'économie ---
    public string RecommendedEnvModel { get; set; } = string.Empty;
    public string RecommendedEnvComplexity { get; set; } = string.Empty;
    public double RecommendedEnvEnergyKwh { get; set; }
    public double RecommendedEnvWaterLiters { get; set; }
    public double RecommendedEnvCostUsd { get; set; }



    public string RecommendedEcoModel { get; set; } = string.Empty;
    public string RecommendedEcoComplexity { get; set; } = string.Empty;
    public double RecommendedEcoEnergyKwh { get; set; } // NOUVEAU
    public double RecommendedEcoWaterLiters { get; set; } // NOUVEAU
    public double RecommendedEcoCostUsd { get; set; }


    public string RecommendedQualityModel { get; set; } = string.Empty;
    public string RecommendedQualityComplexity { get; set; } = string.Empty;
    public double RecommendedQualityEnergyKwh { get; set; }
    public double RecommendedQualityWaterLiters { get; set; }
    public double RecommendedQualityCostUsd { get; set; }
   
    // --- Les  notes et le verdict ---
    public int EfficiencyRating { get; set; }
    public int EnvironmentalRating { get; set; }
    public int EconomicRating { get; set; }
    public int RiskRating { get; set; }
    public string VerdictLevel { get; set; } = string.Empty;

    //  TEMPORAIRE : On garde IsApproved pour ne pas casser tes anciens tests
    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; }
}



public class EvaluationRequestDto
{
    public required string WorkflowDescription { get; set; }
    public int RunFrequency { get; set; }
    public int EmployeeCount { get; set; }
    public double HoursPerRun { get; set; }
    public required string ExperienceLevel { get; set; }
    
    // NOUVEAUX CHAMPS FRONT
    public required string AiModel { get; set; } // "GPT", "Claude", "DeepSeek"
    public required string Complexity { get; set; } // "petit", "grand"

    public long InputTokens { get; set; }
    public long OutputTokens { get; set; }
    public double AiSavingsFraction { get; set; }
    public required string DataSensitivity { get; set; }
    public required string LegalRisk { get; set; }
    public required string UseCase { get; set; } 

}

public class EvaluationResultDto
{
    // Le Verdict
    public string VerdictLevel { get; set; } = string.Empty; 
    public string VerdictReason { get; set; } = string.Empty;
    public string GateTriggered { get; set; } = string.Empty;
    public int EvaluationId { get; set; }

    // Les 4 Notes (1 à 5)
    public int EfficiencyRating { get; set; }
    public int EnvironmentalRating { get; set; }
    public int EconomicRating { get; set; }
    public int RiskRating { get; set; }

    // Impact chiffré
    public double TotalEnergyKwh { get; set; }
    public double TotalCarbonKg { get; set; }
    public double TotalWaterLiters { get; set; }
    public double TotalCostUsd { get; set; }
    public double ValueSavedEur { get; set; }


    public string RecommendedEnvModel { get; set; } = string.Empty;
    public string RecommendedEnvComplexity { get; set; } = string.Empty;
    public double RecommendedEnvEnergyKwh { get; set; }
    public double RecommendedEnvWaterLiters { get; set; }
    public double RecommendedEnvCostUsd { get; set; } // NOUVEAU


    public string RecommendedEcoModel { get; set; } = string.Empty;
    public string RecommendedEcoComplexity { get; set; } = string.Empty;
    public double RecommendedEcoEnergyKwh { get; set; } // NOUVEAU
    public double RecommendedEcoWaterLiters { get; set; } // NOUVEAU
    public double RecommendedEcoCostUsd { get; set; }


    public string RecommendedQualityModel { get; set; } = string.Empty;
    public string RecommendedQualityComplexity { get; set; } = string.Empty;
    public double RecommendedQualityEnergyKwh { get; set; }
    public double RecommendedQualityWaterLiters { get; set; }
    public double RecommendedQualityCostUsd { get; set; }

}

public class EvaluationAiScoreDto
{
 // on ne met que la note de l'IA, pas l'id de l'évaluation, car l'id est passé dans l'URL (question de sécurité et d'optimisation (RESTful design)
    public string AiScore { get; set; } = string.Empty;

}


