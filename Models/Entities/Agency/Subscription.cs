using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Entities.Agency;

namespace TicketingSystem.Models.Entities.Agency;

public class Subscription : BaseEntity
{
    public required string PlanName { get; set; }
    public decimal Price { get; set; }
    public string? Features { get; set; }

    public ICollection<Agency>? Agencies { get; set; }
}
