using Microsoft.AspNetCore.Identity;
using TicketingSystem.Models.Common.BaseEntity;

namespace TicketingSystem.Models.Identity;

public class Role : IdentityRole<Ulid>, IEntity<Ulid>
{
    public Role()
    {
        Id = Ulid.NewUlid();
    }
}
