using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Models.Tickets;

namespace TicketingSystem.Models.Categorys;
    public class Category
    {
        public required int Id { get; set; } // Primary Key
        public required string Name { get; set; }
        public required string Description { get; set; } 
        
        // Foreign Key
        public required int AgencyId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }

        public ICollection<Ticket> Tickets { get; set; }

        public Agency Agency { get; set; }
        public Ticket Ticket { get; set; }
    }