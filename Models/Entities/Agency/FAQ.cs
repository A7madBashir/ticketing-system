using System;
using TicketingSystem.Models.Common.BaseEntity;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystem.Models.Entities.Agency;

namespace TicketingSystem.Models.FAQs;

    public class FAQ : BaseEntity
    {
        // Foreign Key
        public required int AgencyId { get; set; }

        public required string Question { get; set; }
        public required string Answer { get; set; }

        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }

        public Agency Agency { get; set; }
    }