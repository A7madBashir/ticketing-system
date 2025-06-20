namespace TicketingSystem.Models.DTO.Responses.User;

public class UserResponse : BaseResponse
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public new DateTime? CreateTime { get; set; }
    public DateTime? LastLoginAt { get; set; }
}
