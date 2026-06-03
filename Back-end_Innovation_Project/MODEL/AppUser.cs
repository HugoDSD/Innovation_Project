using Microsoft.AspNetCore.Identity;

namespace Back_end_Innovation_Project.MODEL
{
    public class AppUser : IdentityUser
    {
        
        public  string  Name { get; set; }
        public  string Surname { get; set; }
        public string? ProfilePictureUrl { get; set; }          // Non obligatoire, peut être null si l'utilisateur n'a pas de photo de profil
        public string? CompanyName { get; set; }                // A implementer plus tard, permet de classer directement les utilisateurs par entreprise pour faire des stats
        public List<EvaluationHistory> Histories { get; set; } = new();
    }
}