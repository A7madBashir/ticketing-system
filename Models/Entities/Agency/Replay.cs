using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Identity;

namespace TicketingSystem.Models.Entities;

    public class Reply : BaseEntity
    {
        public int Id { get; set; } // Primary Key

        // Foreign Keys
        public required Ulid TicketId { get; set; }
        public required int UserId { get; set; }

        public required string Content { get; set; }
        public required bool IsInternal { get; set; }
        public required bool IsChatbotReply { get; set; }

        public required DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public Ticket Ticket { get; set; }
        
    }