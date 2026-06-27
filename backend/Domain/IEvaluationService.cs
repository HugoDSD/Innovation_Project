namespace Back_end_Innovation_Project.Logic.Interfaces;
using Back_end_Innovation_Project.App.DTOs;

public interface IEvaluationService
{
    Task<(bool Success, IEnumerable<EvaluationHistoryDTO> History, IEnumerable<string> Errors)> GetUserHistoryAsync(
        string userId,
        double? minCarbon = null,
        double? maxCarbon = null,
        string? aiScore = null,
        DateTime? startDate = null,
        DateTime? endDate = null);



    Task<EvaluationResultDto> EvaluateProjectAsync(EvaluationRequestDto request, string userId);

    Task<EvaluationResultDto> EvaluateAiScoreAsync(int evaluationId, EvaluationAiScoreDto request, string userId);

}
