using System;
using TicketingSystem.Models.DTO.Responses.User;
using TicketingSystem.Models.DTO.Responses.Ticket;

namespace TicketingSystem.Models.DTO.Responses.Replies
{
    public class ReplyResponse : BaseResponse
    {
        public ReplyResponse() { }
        public Ulid Id { get; set; }
        public Ulid TicketId { get; set; }
        public UserResponse? User { get; set; }
        public TicketResponse? Ticket { get; set; }
        public string Content { get; set; }
        public bool IsInternal { get; set; }
        public bool IsChatbotReply { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
