using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.Entities;

namespace TicketingSystem.Models.Entities;

    public class Integration : BaseEntity
    {
        public required Ulid Id { get; set; } // Primary Key
        public required string Name { get; set; }
        public required string ApiKeyValue { get; set; }

        // Foreign Key
        public required int AgencyId { get; set; }
        public required DateTime CreatedAt { get; set; }

        public Agency Agency { get; set; }
    }