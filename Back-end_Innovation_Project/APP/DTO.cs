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

    // --- Les métriques physiques ---
    public double CarbonFootprint { get; set; }
    public double WaterFootprintLiters { get; set; }
    public double EnergyKwh { get; set; }
    public double CostUsd { get; set; }
    public double ValueSavedEur { get; set; } // Remplace "HoursSaved"

    // --- Les nouvelles notes et le verdict ---
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
    // --- PARTIE DÉFINITIVE ---
    public required string WorkflowDescription { get; set; }
    public int RunFrequency { get; set; }
    public int EmployeeCount { get; set; }
    public double HoursPerRun { get; set; }
    public required string ExperienceLevel { get; set; } // "junior", "confirmé", "senior", "expert"
    public required string AiModel { get; set; }
    public required string CloudProvider { get; set; }


    // --- PARTIE TEMP (ap la mise en place de l'API OpenAI/Claude ) ---
    public long InputTokens { get; set; }
    public long OutputTokens { get; set; }
    public double AiSavingsFraction { get; set; } // Fraction (ex: 0.5)
    public required string DataSensitivity { get; set; } // "public", "interne", "confidentiel", "réglementé"
    public required string LegalRisk { get; set; } // "faible", "modéré", "élevé", "critique"
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
}

public class EvaluationAiScoreDto
{
 // on ne met que la note de l'IA, pas l'id de l'évaluation, car l'id est passé dans l'URL (question de sécurité et d'optimisation (RESTful design)
    public string AiScore { get; set; } = string.Empty;

}


