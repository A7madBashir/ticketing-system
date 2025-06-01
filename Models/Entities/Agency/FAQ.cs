using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Entities.Agency;

namespace TicketingSystem.Models.FAQs;

public class FAQ : BaseEntity
{
    public Ulid AgencyId { get; set; }
    public string? Question { get; set; }
    public string? Answer { get; set; }

    public virtual Agency? Agency { get; set; }
}
