using Microsoft.AspNetCore.Identity;
using Back_end_Innovation_Project.MODEL;
using Back_end_Innovation_Project.LOGIC.Interfaces;

namespace Back_end_Innovation_Project.LOGIC.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;

    public AuthService(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
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

        // Si ça échoue, on extrait les descriptions des erreurs
        var errors = result.Errors.Select(e => e.Description);
        return (false, errors);
    }
}