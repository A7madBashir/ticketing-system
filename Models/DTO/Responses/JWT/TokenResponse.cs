namespace TicketingSystem.Models.DTO.Responses.JWT;

public class TokenResponse
{
    public bool Succeeded { get; set; }
    public required string Token { get; set; }
    public DateTime TokenValidTo { get; set; }
}
