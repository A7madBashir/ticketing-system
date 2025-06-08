using TicketingSystem.Models.DTO.Requests;
using TicketingSystem.Models.DTO.Responses.User;

namespace TicketingSystem.Models.DTO.Responses.Agency;

public class AgencyResponse
{
    public string Id { get; set; }
    public string Domain { get; set; }
    public string Name { get; set; }
    public DateTime CreateTime { get; set; }
    public UserResponse Subscription { get; set; }
}
