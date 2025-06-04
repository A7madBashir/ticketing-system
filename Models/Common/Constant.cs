namespace TicketingSystem.Models.Common;

public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";

    // Add any other roles you might have
    public const string Manager = "Manager";
    public const string Agent = "Agent";
}

public static class ErrorCodes
{
    public const string InvalidUlid = "Invalid ulid";
    public const string InvalidModelState = "Invalid model state";
}
