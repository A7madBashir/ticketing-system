using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Identity;
using TicketingSystem.Models.Categorys;
using TicketingSystem.Models.Tickets;
using TicketingSystem.Models.FAQs;
using TicketingSystem.Models.Integrations;

namespace TicketingSystem.Models.Entities.Agency;

public class Agency : BaseEntity
{
    public required string Name { get; set; }
    public required string Domain { get; set; }
    public required Ulid SubscriptionId { get; set; }
    public virtual User Subscription { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<FAQ> FAQs { get; set; } = new List<FAQ>();
    public ICollection<Integration> Integrations { get; set; } = new List<Integration>();
}
