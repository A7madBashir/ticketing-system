using TicketingSystem.Models.DTO.Responses.Agency;
using TicketingSystem.Models.DTO.Responses.Category;
using TicketingSystem.Models.DTO.Responses.User;

namespace TicketingSystem.Models.DTO.Responses.Ticket;

public class TicketResponse
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Priority { get; set; }
    public DateTime CreateTime { get; set; }
    public CategoryResponseDto? Category { get; set; }
    public AgencyResponse? Agency { get; set; }
    public UserResponse? CreatedBy { get; set; }
}
