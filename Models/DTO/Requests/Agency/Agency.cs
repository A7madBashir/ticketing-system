namespace TicketingSystem.Models.DTO.Requests.Agency;

public class CreateAgency
{
    public required string Name { get; set; }
    public required string Domain { get; set; }
    public required string SubscriptionId { get; set; }
}
