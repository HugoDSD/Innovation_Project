using Microsoft.AspNetCore.Identity;

namespace InnovationProject.Models
{
    public class AppUser : IdentityUser
    {
        
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }          // Optional, can be null if the user has no profile picture; may be implemented later
        public string? CompanyName { get; set; }                // Optional, can be null if the user has no company name; may be implemented later
        public List<EvaluationHistory> Histories { get; set; } = new();
    }
}