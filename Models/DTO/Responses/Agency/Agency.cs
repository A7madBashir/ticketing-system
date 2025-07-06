using TicketingSystem.Models.Common; // Assuming BaseResponse is in this namespace
using TicketingSystem.Models.DTO.Responses.Subscriptions; // Added this using directive

namespace TicketingSystem.Models.DTO.Responses.Agency;

public class AgencyResponse : BaseResponse
{
    public string Domain { get; set; }
    public string Name { get; set; }
    public SubscriptionResponse? Subscription { get; set; }
    public Ulid? SubscriptionId { get; set; }
}
