using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Entities.Agency;

namespace TicketingSystem.Models.Subscriptions;

public class Subscription : BaseEntity
{
    public required string PlanName { get; set; }
    public decimal Price { get; set; }
    public string? Features { get; set; }

    public ICollection<Agency>? Agencies { get; set; }
}
