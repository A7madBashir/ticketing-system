namespace TicketingSystem.Models.DTO.Responses;

public class PaginatedResponse<TResponse>
    where TResponse : class
{
    public int Draw { get; set; }
    public int RecordsFiltered { get; set; }
    public int RecordsTotal { get; set; }
    public bool HasMoreData { get; set; }
    public required IEnumerable<TResponse> Data { get; set; }
}
