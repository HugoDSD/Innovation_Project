namespace Back_end_Innovation_Project.LOGIC.Interfaces;

public interface IAuthService
{
    // On retourne un booléen et un message (ou une liste d'erreurs)
    Task<(bool Success, IEnumerable<string> Errors)> RegisterUserAsync(string email, string password, string name, string surname, string? companyName);
    Task<(bool Success, string? Token, IEnumerable<string> Errors)> LoginUser(string email, string password);
}