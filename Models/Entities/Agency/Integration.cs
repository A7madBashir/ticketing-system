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
    public required string ApiKeyValue { get; set; }
    public required Ulid AgencyId { get; set; }
    public bool Enabled { get; set; }
    public required DateTime CreatedAt { get; set; }

    public Agency Agency { get; set; }
}
