using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Back_end_Innovation_Project.Logic.Interfaces;
using Back_end_Innovation_Project.App.DTOs;


namespace Back_end_Innovation_Project.App.Controllers;

[ApiController]
[Route("api/Evaluation")]
[Authorize] // Blocks the frontend request if it has no valid token
public class EvalController : ControllerBase
{
    private readonly IEvaluationService _evaluationService;

    public EvalController(IEvaluationService evaluationService)
    {
        _evaluationService = evaluationService;
    }
    

    

    [HttpPost("calculate")]
    public async Task<IActionResult> CalculateImpact(EvaluationRequestDto request)
    {
        // Retrieve the user ID extracted from the token header
        var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Extra safety in case the claim is missing
        if (string.IsNullOrEmpty(userIdFromToken))
        {
            return Unauthorized(new[] { "Impossible d'identifier l'utilisateur à partir du token." });
        }


       var result = await _evaluationService.EvaluateProjectAsync(request, userIdFromToken);

        if (!result.IsApproved && result.Message.StartsWith("ERREUR"))
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    

    
    // The evaluation id is passed in the URL for security and optimization reasons (RESTful design)
    [HttpPut("{RequestId}/score")]
    public async Task<IActionResult> EvaluateAiscore(int RequestId, [FromBody] EvaluationAiScoreDto request)
    {

        if (RequestId <= 0)
        {
            return BadRequest(new[] { "L'ID de la requête est invalide." });
        }

        // 2. Retrieve the user ID extracted from the token, as in the evaluation
        var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdFromToken))
        {
            return Unauthorized(new[] { "Impossible d'identifier l'utilisateur à partir du token." });
        }
        var result = await _evaluationService.EvaluateAiScoreAsync(RequestId, request, userIdFromToken);

        if (!result.IsApproved && (result.Message.StartsWith("ERREUR") || result.Message.Contains("introuvable")))
        {
            return BadRequest(result);
        }

        return Ok(new 
        { 
            isApproved = result.IsApproved,
            evaluationId = result.EvaluationId,
            message = result.Message 
        });
    }

    [HttpGet("history")]
    public async  Task<IActionResult> GetUserHistory( double? minCarbon = null,  double? maxCarbon = null,string? aiScore = null, DateTime? startDate = null, DateTime? endDate = null)
    {
        // Retrieve the user ID extracted from the token, as in the evaluation
        var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdFromToken))
        {
            return Unauthorized(new[] { "Impossible d'identifier l'utilisateur à partir du token." });
        }


        var (success, history, errors) = await _evaluationService.GetUserHistoryAsync(userIdFromToken, minCarbon, maxCarbon, aiScore, startDate, endDate);

        if (!success)
        {
            return BadRequest(errors);
        }
        return Ok(history);     
    }

}

/*
ControllerBase provides:
    - HTTP response handling: it gives you methods like
        Ok() (status 200),
        BadRequest() (status 400),
        Unauthorized() (status 401)
        or NotFound() (status 404).

    - Access to the request context: through ControllerBase you can use User to read the JWT token,
        or access Request and Response to manipulate the HTTP headers.
*/