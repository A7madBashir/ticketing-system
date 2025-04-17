using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticketing-system.Models.FAQ
{
    public class FAQ
    {
        public int Id { get; set; } // Primary Key

        // Foreign Key
        public int AgencyId { get; set; }

        public string Question { get; set; }
        public string Answer { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}