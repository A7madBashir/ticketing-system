using System;
using TicketingSystem.Models.Common.BaseEntity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Models.Entities;

namespace TicketingSystem.Models.Entities;

    public class Category : BaseEntity
    {
        public required Ulid id { get; set; } // Primary Key
        public required string Name { get; set; }
        public required string Description { get; set; } 
        
        // Foreign Key
        public required int AgencyId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }

        public ICollection<Ticket> Tickets { get; set; }

        public Agency Agency { get; set; }
    }