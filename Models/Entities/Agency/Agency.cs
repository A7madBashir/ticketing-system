using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Identity;
using TicketingSystem.Models.Tickets;
using TicketingSystem.Models.Categorys;
using TicketingSystem.Models.FAQs;
using TicketingSystem.Models.Integrations;

namespace TicketingSystem.Models.Entities.Agency;

public class Agency : BaseEntity
{
    public required string Name { get; set; }
    public required string Domain { get; set; }
    public required Ulid SubscriptionId { get; set; }
    public virtual User Subscription { get; set; }

    public ICollection<User> Users { get; set; }
    public ICollection<Ticket> Tickets { get; set; }
    public ICollection<Category> Categories { get; set; }
    public ICollection<FAQ> FAQs { get; set; }
    public ICollection<Integration> Integrations { get; set; }
}
