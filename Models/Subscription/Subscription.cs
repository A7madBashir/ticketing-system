using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticketing-system.Models.Subscription
{
    public class Subscription
    {
        public required int Id { get; set; } // Primary Key
        public required string PlanName { get; set; }
        public required decimal Price { get; set; }

        public required Dictionary string Features { get; set; }

        public required DateTime CreatedAt { get; set; }
    }
}