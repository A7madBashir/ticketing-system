namespace TicketingSystem.Models.DTO.Requests.User;

public class UpdateUserRequest : RegisterUser, IEditRequest<Ulid>
{
    public Ulid Id { get; set; }
}
