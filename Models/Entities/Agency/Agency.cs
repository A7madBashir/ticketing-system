using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Identity;

namespace TicketingSystem.Models.Entities.Agency;

public class Agency : BaseEntity<Ulid>
{
    public required string Name { get; set; }
    public required string Domain { get; set; }
    public required Ulid SubscriptionId { get; set; }
    public virtual User Subscription { get; set; }
}
