using Microsoft.AspNetCore.Identity;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Models.Replys;
using TicketingSystem.Models.Tickets;

namespace TicketingSystem.Models.Identity;

public class User : IdentityUser<Ulid>, IEntity<Ulid>
{
    public User()
    {
        Id = Ulid.NewUlid();
    }

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Gender { get; set; }
    public string Nationality { get; set; }
    public string PassportNumber { get; set; }
    public string Job { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public Ulid AgencyId { get; set; }

    public virtual Agency? Agency { get; set; }

    // Navigation properties for related entities
    public ICollection<Ticket> CreatedTickets { get; set; } // Tickets created by this user
    public ICollection<Ticket> AssignedTickets { get; set; } // Tickets assigned to this user
    public ICollection<Reply> Replies { get; set; }
    public ICollection<Analytic> Analytics { get; set; } // Analytics tracked for this agent
}
