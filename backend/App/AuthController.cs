using Microsoft.AspNetCore.Mvc;
using InnovationProject.Logic.Interfaces;
using InnovationProject.App.DTOs;


namespace InnovationProject.App.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    // _authService is an instance of AuthService (the concrete class implementing IAuthService) used to handle the authentication business logic.
    // Dependency injection lets us obtain an AuthService instance without creating it manually in the controller.
    private readonly IAuthService _authService;

    // Inject OUR service from the LOGIC layer
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


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto request)
    {
        var (success, token, errors) = await _authService.LoginUser(request.Email, request.Password);
        if (success)
        {
            return Ok(new{message = "connexion établie", token =token});
        }
        return Unauthorized(errors);
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