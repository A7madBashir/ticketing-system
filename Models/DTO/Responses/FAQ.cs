using System;

namespace TicketingSystem.Models.DTO.Responses.FAQ
{
    public class FAQResponseDto : BaseResponse
    {
        public Ulid Id { get; set; }
        public Ulid AgencyId { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public string? AgencyName { get; set; }
    }
}
