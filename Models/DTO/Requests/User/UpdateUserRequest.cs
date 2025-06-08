namespace TicketingSystem.Models.DTO.Requests.User;

public class UpdateUserRequest : RegisterUser
{
    public required string Id { get; set; }
}
