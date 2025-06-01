using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Models.Tickets;

namespace TicketingSystem.Models.Categorys;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Ulid AgencyId { get; set; }

    public virtual Agency? Agency { get; set; }

    public ICollection<Ticket>? Tickets { get; set; }
}
