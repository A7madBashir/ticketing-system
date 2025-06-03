using System;

namespace TicketingSystem.Models.DTO.Requests.Category
{
    public class CategoryRequestDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Ulid AgencyId { get; set; }
    }
}
