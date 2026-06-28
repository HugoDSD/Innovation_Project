using System.ComponentModel.DataAnnotations;

namespace InnovationProject.Dtos;

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
