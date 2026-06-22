using Microsoft.AspNetCore.Identity;

namespace Back_end_Innovation_Project.MODEL
{
    public class AppUser : IdentityUser
    {
        
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }          // Non obligatoire, peut être null si l'utilisateur n'a pas de photo de profil, a voir si on veut l'implémenter plus tard
        public string? CompanyName { get; set; }                // Non obligatoire, peut être null si l'utilisateur n'a pas de photo de profil, a voir si on veut l'implémenter plus tard
        public List<EvaluationHistory> Histories { get; set; } = new();
    }
}