namespace InnovationProject.Dtos;

public class EvaluationResultDto
{
    public bool IsApproved { get; set; }
    public int EvaluationId { get; set; }
    public string Message { get; set; } = string.Empty;

    // Environmental impact
    public double TotalEnergyKwh { get; set; }
    public double TotalCarbonKg { get; set; }
    public double TotalWaterLiters { get; set; }

    // Economic impact
    public double TotalCostUsd { get; set; }

    // Social impact
    public double TotalHoursSaved { get; set; }
    public double RiskScore { get; set; }
}
