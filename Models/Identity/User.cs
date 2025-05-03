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

    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Gender { get; set; }
    public required string Nationality { get; set; }
    public required string PassportNumber { get; set; }
    public required string Job { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
    public Ulid AgencyId { get; set; }

    public Agency? Agency { get; set; }

    public ICollection<Ticket>? TicketsCreated { get; set; }
    public ICollection<Ticket>? TicketsAssigned { get; set; }
    public ICollection<Reply>? Replies { get; set; }
}
