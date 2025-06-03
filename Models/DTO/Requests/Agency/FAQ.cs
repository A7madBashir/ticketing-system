using System;

namespace TicketingSystem.Models.DTO.Requests.FAQ
{
    public class FAQRequestDto
    {
        public Ulid AgencyId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
    }
}
