using System.ComponentModel.DataAnnotations;

namespace InnovationProject.Dtos;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [MinLength(8)]
    public required string Password { get; set; }

    public required string Name { get; set; }
    public required string Surname { get; set; }

    public string? CompanyName { get; set; }
}
