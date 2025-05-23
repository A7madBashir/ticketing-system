using TicketingSystem.Models.DTO.User;

namespace TicketingSystem.Models.DTO.Responses.Agency;

public class Agency
{
    public string Id { get; set; }
    public string Domain { get; set; }
    public string Name { get; set; }
    public UserProfile Subscription { get; set; }
}
