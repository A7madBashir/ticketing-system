using System;
using TicketingSystem.Models.DTO.Responses.Ticket;
using TicketingSystem.Models.DTO.Responses.User;

namespace TicketingSystem.Models.DTO.Responses.Replies;

public class ReplyResponse : BaseResponse
{
    public Ulid TicketId { get; set; }
    public UserResponse? User { get; set; }
    public TicketResponse? Ticket { get; set; }
    public string Type { get; set; }
    public string Content { get; set; }
}
