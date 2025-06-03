using System;

namespace TicketingSystem.Models.DTO.Responses.Category
{
    public class CategoryResponseDto
    {
        public Ulid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Ulid AgencyId { get; set; }
        public string? AgencyName { get; set; }
        public int TicketCount { get; set; }
    }
}
