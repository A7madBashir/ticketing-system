using System;
using TicketingSystem.Models.DTO.Responses.Agency;

namespace TicketingSystem.Models.DTO.Responses.Category
{
    public class CategoryResponseDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
        public string? Description { get; set; }
        public AgencyResponse? Agency { get; set; }
        public int? TicketCount { get; set; }
    }
}
