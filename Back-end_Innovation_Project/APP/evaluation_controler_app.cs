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
        //  On récupère l'ID utilisateur extrait du header du token
        var userIdFromToken = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        // Sécurité supplémentaire au cas où le claim serait introuvable
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

    

    
    // l'id de l'évaluation est passé dans l'URL pour des raisons de sécurité et d'optimisation (RESTful design)
    [HttpPut("{RequestId}/score")]
    public async Task<IActionResult> EvaluateAiscore(int RequestId, [FromBody] EvaluationAiScoreDto request)
    {

        if (RequestId <= 0)
        {
            return BadRequest(new[] { "L'ID de la requête est invalide." });
        }

        // 2. On récupère l'ID utilisateur extrait du Token comme pour l'evaluation
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
        //  On récupère l'ID utilisateur extrait du Token comme pour l'evaluation
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