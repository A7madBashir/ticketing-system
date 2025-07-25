using System.ComponentModel.DataAnnotations;

namespace TicketingSystem.Models.DTO.Requests.Ticket;

public class CreateAnonymousTicket
{
    public required string Name { get; set; }
    [EmailAddress]
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public required string Content { get; set; }
}
