using System.Text.Json.Serialization;

namespace TicketingSystem.Models.DTO.Requests.Replies
{
    public class CreateReplyRequest : ICreateRequest
    {
        public Ulid TicketId { get; set; }
        public Ulid UserId { get; set; }
        public required string Content { get; set; }
        public bool IsInternal { get; set; }
        public bool IsChatbotReply { get; set; }
    }

    public class EditReplyRequest : CreateReplyRequest, IEditRequest<Ulid>
    {
       [JsonIgnore]
       public Ulid Id { get; set; }
    }
}
