using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Identity;
using TicketingSystem.Models.Tickets;

namespace TicketingSystem.Models.Replys;

public class Reply : BaseEntity
{
    public Ulid TicketId { get; set; }
    public Ulid UserId { get; set; }
    public required string Content { get; set; }
    public bool IsInternal { get; set; }
    public bool IsChatbotReply { get; set; }

    public virtual User? User { get; set; }
    public virtual Ticket? Ticket { get; set; }
}
