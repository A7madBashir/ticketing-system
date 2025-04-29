using Microsoft.AspNetCore.Identity;
using TicketingSystem.Models.Common.BaseEntity;

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
}
