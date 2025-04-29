using System;
using System.Collections.Generic;
using System.Linq;
using TicketingSystem.Models.Common.BaseEntity;
using System.Threading.Tasks;
using TicketingSystem.Models.Entities;
using TicketingSystem.Models.Identity;

namespace TicketingSystem.Models.Entities;

    public class Ticket : BaseEntity
    {

        public required Ulid Id { get; set; } // Primary Key
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Status { get; set; }
        public required string Priority { get; set; }

        // Foreign Keys
        public required int CategoryId { get; set; }
        public required int AssignedTo { get; set; }
        public required int CreatedBy { get; set; }
        public required int AgencyId { get; set; }

        public required bool OriginatedFromChatbot { get; set; }

        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }

        public ICollection<Reply> Replies { get; set; }


        public Agency Agency { get; set; }
        public User CreatedByUser { get; set; }
        public User AssignedToUser { get; set; }
        public Category Category { get; set; }
        

    }