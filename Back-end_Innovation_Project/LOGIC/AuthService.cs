using Microsoft.AspNetCore.Identity;
using Back_end_Innovation_Project.MODEL;
using Back_end_Innovation_Project.LOGIC.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;



namespace Back_end_Innovation_Project.LOGIC.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration; // reads the configuration file (appsettings.json) to retrieve the JWT secret key

    public AuthService(UserManager<AppUser> userManager, IConfiguration configuration  )
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<(bool Success, IEnumerable<string> Errors)> RegisterUserAsync(string email, string password, string name, string surname, string? companyName)
    {
        var user = new AppUser
        {
            UserName = email,
            Email = email,
            Name = name,
            Surname = surname,
            CompanyName = companyName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            return (true, Array.Empty<string>());
        }

        // On failure, extract the error descriptions
        var errors = result.Errors.Select(e => e.Description);
        return (false, errors);
    }

    public async Task<(bool Success, string? Token, IEnumerable<string> Errors)> LoginUser(string email, string password)
    {
       var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return (false,null,new[] { "Email d'utilisateur non trouvé" });
        }
        var result = await _userManager.CheckPasswordAsync(user, password);
        if (!result)
        {
            return (false, null, new[] { "Mot de passe incorrect" });
        }
        
        
        
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(ClaimTypes.GivenName, user.Name),
            new Claim(ClaimTypes.Surname, user.Surname),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };



        var secretKey = _configuration["JwtSettings:Secret"];
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new Exception("The JWT secret key is not configured. Ask Hugo for the appsettings.json file.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(2), 
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenObject = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(tokenObject);

        return (true, tokenString, Array.Empty<string>());
            
    }

    
}