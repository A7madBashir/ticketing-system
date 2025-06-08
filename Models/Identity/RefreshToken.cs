using TicketingSystem.Models.Common.BaseEntity;

namespace TicketingSystem.Models.Identity;

public class RefreshToken : BaseEntity
{
    public virtual User? User { get; set; }
    public required Ulid UserId { get; set; }
    public required string Token { get; set; }
    public DateTime Expires { get; set; }
    public required string CreatedByIp { get; set; }
    public DateTime? Revoked { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires;
    public bool IsRevoked => Revoked != null;
    public bool IsActive => !IsRevoked && !IsExpired;
}
