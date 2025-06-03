using System;

namespace TicketingSystem.Models.DTO.Requests.Replies
{
    public class CreateReplyRequest
    {
        public Ulid TicketId { get; set; }
        public Ulid UserId { get; set; }
        public required string Content { get; set; }
        public bool IsInternal { get; set; }
        public bool IsChatbotReply { get; set; }
    }

    public class UpdateReplyRequest
    {
        public required Ulid Id { get; set; }
        public required string Content { get; set; }
        public bool IsInternal { get; set; }
        public bool IsChatbotReply { get; set; }
    }
}
