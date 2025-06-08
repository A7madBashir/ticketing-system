namespace TicketingSystem.Settings;

public class TokenSettings
{
    public string Issuer { get; set; }
    public string Secret { get; set; }
    public string Audience { get; set; }
    public int RefreshTokeExpireInMinutes { get; set; }
    public int ExpiryInMinutes { get; set; }
}
