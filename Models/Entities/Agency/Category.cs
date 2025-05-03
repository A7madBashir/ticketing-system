using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Entities;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Models.Tickets;

namespace TicketingSystem.Models.Categorys;

public class Category : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required Ulid AgencyId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Agency? Agency { get; set; }

    public ICollection<Ticket>? Tickets { get; set; }
}
