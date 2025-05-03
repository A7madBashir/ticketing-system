using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Models.Categorys;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Models.Identity;
using TicketingSystem.Models.Replys;

namespace TicketingSystem.Models.Tickets;

public class Ticket : BaseEntity
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Status { get; set; }
    public required string Priority { get; set; }
    public required Ulid CategoryId { get; set; }
    public required Ulid AssignedTo { get; set; }
    public required Ulid CreatedBy { get; set; }
    public required Ulid AgencyId { get; set; }
    public required bool OriginatedFromChatbot { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Agency? Agency { get; set; }
    public User? CreatedByUser { get; set; }
    public User? AssignedToUser { get; set; }
    public Category? Category { get; set; }

    public ICollection<Reply>? Replies { get; set; }
}
