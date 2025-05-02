using System;
using TicketingSystem.Models.Common.BaseEntity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Models.Identity;
using TicketingSystem.Models.Tickets;

namespace TicketingSystem.Models.Replays;

    public class Replay : BaseEntity
    {

        // Foreign Keys
        public required Ulid TicketId { get; set; }
        public required Ulid UserId { get; set; }

        public required string Content { get; set; }
        public required bool IsInternal { get; set; }
        public required bool IsChatbotReply { get; set; }

        public required DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public Ticket Ticket { get; set; }
        
    }