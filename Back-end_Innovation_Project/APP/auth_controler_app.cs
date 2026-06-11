using Microsoft.AspNetCore.Mvc;
using Back_end_Innovation_Project.LOGIC.Interfaces;
using Back_end_Innovation_Project.APP.DTOs;

namespace Back_end_Innovation_Project.APP.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    //_authService est une instance de la classe AuthService (la classe concrète qui implémente notre interface IAuthService) que nous allons utiliser pour gérer la logique métier de l'authentification. 
    // C'est grâce à l'injection de dépendances que nous pouvons obtenir une instance de AuthService sans avoir à la créer manuellement dans notre contrôleur.
    private readonly IAuthService _authService;

    // On injecte NOTRE service de la couche LOGIC
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto request)
    {
        var result = await _authService.RegisterUserAsync(
            request.Email, 
            request.Password, 
            request.Name, 
            request.Surname, 
            request.CompanyName);

        if (result.Success)
        {
            return Ok(new { message = "Utilisateur créé" });
        }

        return BadRequest(result.Errors);
    }
}