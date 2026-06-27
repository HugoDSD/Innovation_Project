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

    // --- Les métriques environnementales ---
    public double CarbonFootprint { get; set; }
    public double WaterFootprintLiters { get; set; }
    public double EnergyKwh { get; set; }

       public double CostUsd { get; set; }
    public double HoursSaved { get; set; }
    public double RiskScore { get; set; }


    public bool IsApproved { get; set; }

    public DateTime CreatedAt { get; set; }
}



public class EvaluationRequestDto
{
    public required string ModelName { get; set; }
    public required string Provider { get; set; } 
    public long InputTokens { get; set; }
    public long OutputTokens { get; set; }

    public double HoursSavedReports { get; set; }
    public double HoursSavedImages { get; set; }
    public double HoursSavedPresentations { get; set; }


    [Range(1, 5)]
    public int DataSensitivity { get; set; }
    
    [Range(1, 5)]
    public int LegalRisk { get; set; }
}


public class EvaluationResultDto
{
    public bool IsApproved { get; set; }
    public int EvaluationId { get; set; }
    public string Message { get; set; } = string.Empty;
    
    // Impact Environnemental
    public double TotalEnergyKwh { get; set; }
    public double TotalCarbonKg { get; set; }
    public double TotalWaterLiters { get; set; }
    
    // Impact Économique
    public double TotalCostUsd { get; set; }
    
    // Impact Social
    public double TotalHoursSaved { get; set; }
    public double RiskScore { get; set; }
}

public class EvaluationAiScoreDto
{
 // on ne met que la note de l'IA, pas l'id de l'évaluation, car l'id est passé dans l'URL (question de sécurité et d'optimisation (RESTful design)
    public string AiScore { get; set; } = string.Empty;

}


