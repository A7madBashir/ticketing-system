using System.Text.Json.Serialization;

namespace TicketingSystem.Models.DTO.Requests.Replies
{
    public class CreateReplyRequest : ICreateRequest
    {
        public string TicketId { get; set; }
        public string UserId { get; set; }
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
