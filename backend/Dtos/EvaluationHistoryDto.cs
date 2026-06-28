namespace InnovationProject.Dtos;

public class EvaluationHistoryDto
{
    public string Id { get; set; } = string.Empty;

    public string ModelName { get; set; } = string.Empty;
    public string AiScore { get; set; } = string.Empty;

    // --- Environmental metrics ---
    public double CarbonFootprint { get; set; }
    public double WaterFootprintLiters { get; set; }
    public double EnergyKwh { get; set; }

    public double CostUsd { get; set; }
    public double HoursSaved { get; set; }
    public double RiskScore { get; set; }

    public bool IsApproved { get; set; }

    public DateTime CreatedAt { get; set; }
}
