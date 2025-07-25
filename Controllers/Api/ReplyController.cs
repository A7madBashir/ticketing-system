using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using TicketingSystem.Models.DTO.Requests;
using TicketingSystem.Models.DTO.Requests.Replies;
using TicketingSystem.Models.DTO.Responses.Replies;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Services;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

// Use C# 12 primary constructor for repository and mapper directly
public class ReplyController(
    IReplyRepository repository,
    ITicketRepository ticketRepository,
    IIdentityService identityService, // Renamed for consistency with field
    Mapper mapper
)
    : CrudController<
        Reply,
        Ulid,
        ReplyResponse, // Corrected to ReplyResponse (not ReplyResponseDto)
        CreateReplyRequest,
        EditReplyRequest
    >(repository, mapper)
{
    // These fields are now redundant with C# 12 primary constructors,
    // as 'repository' and 'ticketRepository' are directly accessible within the class scope.
    // Keeping them for clarity if you prefer, but they are not strictly necessary.
    private readonly IReplyRepository _repository = repository;
    private readonly ITicketRepository _ticketRepository = ticketRepository; // Renamed to match constructor param
    private readonly IIdentityService _identityService = identityService;

    protected override async Task<IQueryable<Reply>?> BuildBaseQuery(DataTableRequest req)
    {
        // Get the base query from the repository (which typically returns an AsNoTracking query)
        var query = _repository.Query();

        // Always include Ticket and User navigation properties for replies
        // This ensures related data is loaded for display and mapping
        query = query.Include(r => r.Ticket).Include(r => r.User); // <--- Added include for User

        var currentUser = (await _identityService.GetUser(User))!;

        var isAdmin = await _identityService.IsAdmin(currentUser);
        var IsAgent = await _identityService.IsAgent(currentUser);

        if (IsAgent && !isAdmin)
        {
            var agencyUlid = currentUser.AgencyId;
            if (agencyUlid is null)
            {
                throw new ArgumentNullException("Agent not authorized");
            }
            else
            {
                query = query.Where(q => q.Ticket!.AgencyId == agencyUlid);
            }
        }

        if (req.Filters.Count > 0)
        {
            // Safely extract the TicketId filter.
            // Using OfType<string>() ensures we only try to parse string values if Value is object.
            // A safer approach might be to ensure Filters are KeyValuePair<string, string> or use a custom filter DTO.
            var ticketIdFilter = req.Filters.FirstOrDefault(r => r.Key == "TicketId");

            if (ticketIdFilter.Key == "TicketId" && !string.IsNullOrEmpty(ticketIdFilter.Value))
            {
                if (Ulid.TryParse(ticketIdFilter.Value, out Ulid parsedTicketUlid)) // Renamed for clarity
                {
                    query = query.Where(r => r.TicketId == parsedTicketUlid);
                }
                // Optional: You might want to log an error or return null if parsing fails for a provided filter
            }
        }

        return query;
    }

    protected override async Task<OneOf<Success, Error<string>>> BeforeCreateAsync(
        CreateReplyRequest createDto
    )
    {
        bool validTicketId = Ulid.TryParse(createDto.TicketId, out Ulid TicketUlid);
        if (!validTicketId)
        {
            return new Error<string>("Invalid Ticket ulid");
        }

        if (!await _ticketRepository.ExistAsync(TicketUlid))
        {
            return new Error<string>("TicketId not exist");
        }

        var currentUser = (await _identityService.GetUser(User))!;
        createDto.UserId = currentUser.Id.ToString();

        var isAdmin = await _identityService.IsAdmin(currentUser);
        var IsAgent = await _identityService.IsAgent(currentUser);

        if (IsAgent && !isAdmin)
        {
            var userAgencyId = currentUser.AgencyId;
            if (userAgencyId is null)
            {
                return new Error<string>("Agent not authorized");
            }

            var ticketAgencyId = (
                await _ticketRepository.FirstOrDefaultAsync(r => r.Id == TicketUlid)
            )!.AgencyId;

            if (ticketAgencyId != userAgencyId)
            {
                return new Error<string>("Agent not authorized agency ulid");
            }
        }

        return new Success();
    }

    protected override string[] GetSearchableProperties()
    {
        // Reply.Content is a good searchable property.
        // Reply.IsInternal (boolean) is usually not useful for generic text search.
        // Consider including searchable properties from related entities if included (e.g., User.UserName).
        return new[]
        {
            nameof(Reply.Content),
            // nameof(Reply.User.UserName) // Example if you want to search by user name (requires User to be included)
        };
    }

    protected override string[] IncludeNavigation()
    {
        // This method is typically used by the base CrudController to automatically
        // include navigation properties. If you've already handled inclusions in
        // BuildBaseQuery, and your CrudController's Get methods call BuildBaseQuery,
        // then this might be redundant or you might list additional navigation properties here.

        // It should contain names of navigation properties (e.g., nameof(Reply.Ticket)), not foreign keys (TicketId).
        // If your base CrudController uses this, you'd put:
        return new[] { nameof(Reply.Ticket), nameof(Reply.User) };
        // If BuildBaseQuery handles all necessary includes, this method might return an empty array or null,
        // depending on how your CrudController interprets it.
    }
}
