using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Models.DTO.Requests.User;

public class RegisterUser : ICreateRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }
    public string? AgencyId { get; set; }

    [Phone]
    public string PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public string? Nationality { get; set; }
    public string? PassportNumber { get; set; }
    public string? Job { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public string Password { get; set; }

    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
