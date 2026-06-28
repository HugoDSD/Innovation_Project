namespace InnovationProject.Interfaces;
using InnovationProject.Dtos;

public interface IEvaluationService
{
    Task<(bool Success, IEnumerable<EvaluationHistoryDto> History, IEnumerable<string> Errors)> GetUserHistoryAsync(
        string userId,
        double? minCarbon = null,
        double? maxCarbon = null,
        string? aiScore = null,
        DateTime? startDate = null,
        DateTime? endDate = null);



    Task<EvaluationResultDto> EvaluateProjectAsync(EvaluationRequestDto request, string userId);

    Task<EvaluationResultDto> EvaluateAiScoreAsync(int evaluationId, EvaluationAiScoreDto request, string userId);

}
