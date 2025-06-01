using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Entities.Agency;

namespace TicketingSystem.Models.Integrations;

public class Integration : BaseEntity
{
    public required string Name { get; set; }
    public string? ApiKey { get; set; }
    public Ulid AgencyId { get; set; }
    public bool Enabled { get; set; }

    public virtual Agency? Agency { get; set; }
}
