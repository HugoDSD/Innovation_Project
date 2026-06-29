namespace Back_end_Innovation_Project.LOGIC.Interfaces;
using Back_end_Innovation_Project.APP.DTOs;
public interface IAuthService
{
    // On retourne un booléen et un message (ou une liste d'erreurs)
    Task<(bool Success, IEnumerable<string> Errors)> RegisterUserAsync(string email, string password, string name, string surname, string? companyName);
    Task<(bool Success, string? Token, IEnumerable<string> Errors)> LoginUser(string email, string password);
    
}

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

    