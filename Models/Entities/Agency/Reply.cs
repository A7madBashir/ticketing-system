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
    public required Ulid TicketId { get; set; }
    public required Ulid UserId { get; set; }
    public required string Content { get; set; }
    public required bool IsInternal { get; set; }
    public required bool IsChatbotReply { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public Ticket? Ticket { get; set; }
}
