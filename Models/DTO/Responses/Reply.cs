using System;

namespace TicketingSystem.Models.DTO.Responses.Replies
{
    public class ReplyResponse
    {
        public Ulid Id { get; set; }
        public Ulid TicketId { get; set; }
        public Ulid UserId { get; set; }
        public string Content { get; set; }
        public bool IsInternal { get; set; }
        public bool IsChatbotReply { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
