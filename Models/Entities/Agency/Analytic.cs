using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Identity;

namespace TicketingSystem.Models.Entities.Agency;

public class Analytic : BaseEntity
{
    // Foreign Keys
    public Ulid AgentId { get; set; } // Refers to a User
    public Ulid AgencyId { get; set; }

    // Navigation properties
    public virtual User? Agent { get; set; } // Navigation for the agent being tracked
    public virtual Agency? Agency { get; set; }

    public int TicketsResolved { get; set; }

    // `interval` in PostgreSQL can be mapped to TimeSpan in C#
    public TimeSpan AverageResponseTime { get; set; }
    public float CustomerSatisfactionScore { get; set; } // float is fine for float/real

    public Analytic() { }
}
