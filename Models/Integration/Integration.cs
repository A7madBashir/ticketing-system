using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticketing-system.Models.Integration
{
    public class Integration
    {
        public int Id { get; set; } // Primary Key
        public string Name { get; set; }
        public string ApiKeyValue { get; set; }

        // Foreign Key
        public int AgencyId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}