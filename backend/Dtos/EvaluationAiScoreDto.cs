namespace InnovationProject.Dtos;

public class EvaluationAiScoreDto
{
    // Only the AI score is included, not the evaluation id, since the id is passed in the URL (for security and optimization — RESTful design)
    public string AiScore { get; set; } = string.Empty;
}
