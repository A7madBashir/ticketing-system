using Microsoft.AspNetCore.Identity;

namespace TicketingSystem.Models.DTO.Responses.User;

public class LoginResponse
{
    public bool Succeeded { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime TokenValidTo { get; set; }
    public DateTime RefreshTokenValidTo { get; set; }
    public IEnumerable<IdentityError>? Errors { get; set; }
}
