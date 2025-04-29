using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Entities;

namespace TicketingSystem.Models.Entities;

public class Subscription : BaseEntity
{
    public required Ulid Id { get; set; } // Primary Key
    public required string PlanName { get; set; }
    public required decimal Price { get; set; }

    public required string Features { get; set; }

    public required DateTime CreatedAt { get; set; }

    public Agency Agency { get; set; }
}
