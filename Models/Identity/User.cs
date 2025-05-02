using Microsoft.AspNetCore.Identity;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Tickets;
using TicketingSystem.Models.Replays;
using TicketingSystem.Models.Entities.Agency;


namespace TicketingSystem.Models.Identity;

public class User : IdentityUser<Ulid>, IEntity<Ulid>
{
    public User()
    {
        Id = Ulid.NewUlid();
    }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public string? Nationality { get; set; }
    public string? PassportNumber { get; set; }
    public string? Job { get; set; }
    public DateTime DateOfBirth { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }


    public ICollection<Ticket> TicketsCreated { get; set; }
    public ICollection<Ticket> TicketsAssigned { get; set; }
    public ICollection<Replay> Replies { get; set; }

    public Ulid AgencyId { get; set; }
    public Agency Agency { get; set; }    

}
