using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Back_end_Innovation_Project.LOGIC.Interfaces;
using Back_end_Innovation_Project.APP.DTOs;


namespace Back_end_Innovation_Project.APP.Controllers;

[ApiController]
[Route("api/Evaluation")]
[Authorize] //Permet de bloquer la requete du front s'il n'a pas de token valide
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
        var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdFromToken))
        {
            return Unauthorized(new[] { "Impossible d'identifier l'utilisateur à partir du token." });
        } 

        try
        {
            var result = await _evaluationService.EvaluateProjectAsync(request, userIdFromToken);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            // On attrape spécifiquement notre erreur de "modèle introuvable"
            return BadRequest(new { message = ex.Message });
        }
    }

    

    
    // l'id de l'évaluation est passé dans l'URL pour des raisons de sécurité et d'optimisation (RESTful design)
    [HttpPut("{RequestId}/score")]
    public async Task<IActionResult> EvaluateAiscore(int RequestId, [FromBody] EvaluationAiScoreDto request)
    {
        if (RequestId <= 0)
        {
            return BadRequest(new[] { "L'ID de la requête est invalide." });
        }

        var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdFromToken))
        {
            return Unauthorized(new[] { "Impossible d'identifier l'utilisateur à partir du token." });
        } 

        // On utilise un Tuple (Success, Message) pour une réponse plus propre
        var (success, message) = await _evaluationService.EvaluateAiScoreAsync(RequestId, request, userIdFromToken);

        if (!success)
        {
            return BadRequest(new { message = message });
        }

        return Ok(new { message = message, evaluationId = RequestId });
    }



    [HttpGet("history")]
    public async Task<IActionResult> GetUserHistory(double? minCarbon = null, double? maxCarbon = null, string? aiScore = null, DateTime? startDate = null, DateTime? endDate = null)
    {
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
ControllerBase permet : 
    - La gestion des réponses HTTP : C'est elle qui te fournit les méthodes comme 
        Ok() (statut 200), 
        BadRequest() (statut 400), 
        Unauthorized() (statut 401) 
        ou NotFound() (statut 404).

    - L'accès au contexte de la requête : C'est grâce à ControllerBase que tu peux taper User pour aller lire le Token JWT,
        ou accéder à Request et Response pour manipuler les en-têtes HTTP.
*/