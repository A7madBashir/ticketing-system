namespace TicketingSystem.Models.DTO.Requests;

public class DataTableRequest
{
    public int Page { get; set; } = 1;
    public int Count { get; set; } = 10;
    public int Draw { get; set; } = 1;

    public string? SortColumn { get; set; }
    public string SortDirection { get; set; } = "asc";
    public bool OrderByDescending => SortDirection?.ToLower() == "desc";

    public string? Search { get; set; }

    public Dictionary<string, string> Filters { get; set; } = new Dictionary<string, string>();
}
