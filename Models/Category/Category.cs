using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ticketing-system.Models.Category
{
    public class Category
    {
        public required int Id { get; set; } // Primary Key
        public required string Name { get; set; }
        public required string Description { get; set; } 
        
        // Foreign Key
        public required int AgencyId { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }

    }
}