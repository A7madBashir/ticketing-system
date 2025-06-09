using TicketingSystem.Models.DTO.Requests;
using TicketingSystem.Models.DTO.Responses.User;

namespace TicketingSystem.Models.DTO.Responses.Agency;

public class AgencyResponse : BaseResponse
{
    public string Domain { get; set; }
    public string Name { get; set; }
    public UserResponse? Subscription { get; set; }
}
