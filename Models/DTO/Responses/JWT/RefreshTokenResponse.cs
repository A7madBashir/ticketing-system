namespace TicketingSystem.Models.DTO.Responses.JWT;

public class RefreshTokenResponse
{
    public bool Succeeded { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime RefreshTokenValidTo { get; set; }
}
