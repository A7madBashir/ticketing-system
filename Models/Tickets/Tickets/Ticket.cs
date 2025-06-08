using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Models.Identity;

namespace TicketingSystem.Models.Tickets;

public class Ticket : BaseEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public Ulid CategoryId { get; set; }
    public Ulid CreatedById { get; set; }
    public Ulid AgencyId { get; set; }
    public bool OriginatedFromChatbot { get; set; }

    public virtual Agency? Agency { get; set; }
    public virtual User? CreatedBy { get; set; }
    public virtual Category? Category { get; set; }

    public ICollection<User>? AssignedTo { get; set; }
    public ICollection<Reply>? Replies { get; set; }
}
