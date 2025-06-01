using TicketingSystem.Models.Categorys;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.FAQs;
using TicketingSystem.Models.Identity;
using TicketingSystem.Models.Integrations;
using TicketingSystem.Models.Subscriptions;
using TicketingSystem.Models.Tickets;

namespace TicketingSystem.Models.Entities.Agency;

public class Agency : BaseEntity
{
    public required string Name { get; set; }
    public string? Domain { get; set; }
    public Ulid SubscriptionId { get; set; }

    public virtual Subscription? Subscription { get; set; }

    public ICollection<User>? Users { get; set; }
    public ICollection<Ticket>? Tickets { get; set; }
    public ICollection<Category>? Categories { get; set; }
    public ICollection<FAQ>? FAQs { get; set; }
    public ICollection<Analytic>? Analytics { get; set; }
    public ICollection<Integration>? Integrations { get; set; }
}
