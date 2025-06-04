namespace TicketingSystem.Models.DTO.Requests.User;

public class UpdateUserRequest : UserRequestDto
{
    public required string Id { get; set; }
}
