using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Entities;

namespace TicketingSystem.Models.Entities;

    public class FAQ : BaseEntity
    {
        public required Ulid Id { get; set; } // Primary Key

        // Foreign Key
        public required int AgencyId { get; set; }

        public required string Question { get; set; }
        public required string Answer { get; set; }

        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }

        public Agency Agency { get; set; }
    }