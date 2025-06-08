namespace TicketingSystem.Models.DTO.Responses;

public class PaginatedResponse<TResponse>
    where TResponse : class
{
    public int Page { get; set; }
    public int Total { get; set; }
    public int Count { get; set; }
    public required IEnumerable<TResponse> Data { get; set; }
}
