using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using TicketingSystem.Filters;
using TicketingSystem.Models.Common;
using TicketingSystem.Models.DTO.Requests;
using TicketingSystem.Models.DTO.Requests.FAQ;
using TicketingSystem.Models.DTO.Responses;
using TicketingSystem.Models.DTO.Responses.FAQ;
using TicketingSystem.Models.FAQs;
using TicketingSystem.Services;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

[Authorize(Policy = AuthenticationPolicy.AgentAccess)]
public class FAQController(
    IFAQRepository repository,
    IAgencyRepository agencyRepository,
    IIdentityService identityService,
    Mapper mapper
) : CrudController<FAQ, Ulid, FAQResponseDto, CreateFAQRequest, EditFAQRequest>(repository, mapper)
{
    private readonly IFAQRepository _repository = repository;
    private readonly IAgencyRepository _agencyRepository = agencyRepository;
    private readonly IIdentityService _identityService = identityService;

    protected override async Task<IQueryable<FAQ>>? BuildBaseQuery(DataTableRequest req)
    {
        // Get the base query from the repository (which typically returns an AsNoTracking query)
        var query = _repository.Query();

        // This ensures that when Agency data is fetched, its associated Subscription data is also loaded.
        query = query.Include(a => a.Agency);

        if (User is not null && User.Identity.IsAuthenticated)
        {
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
                    query = query.Where(q => q.AgencyId == agencyUlid);
                }
            }
        }
        else
        {
            if (CurrentAgencyId != null)
            {
                query = query.Where(q => q.AgencyId == CurrentAgencyId);
            }
        }

        if (req.Filters.Count > 0)
        {
            // Filter by agency id
            string agencyId = req.Filters.Where(r => r.Key == "agencyId").FirstOrDefault().Value;
            if (!string.IsNullOrEmpty(agencyId) && Ulid.TryParse(agencyId, out Ulid subId))
            {
                query = query.Where(r => r.AgencyId == subId);
            }
        }

        return query;
    }

    [AllowAnonymous]
    [ApiKeyAuthorize]
    public override async Task<ActionResult<PaginatedResponse<FAQResponseDto>>> DataTable(
        [FromQuery] DataTableRequest req
    )
    {
        if ((User.Identity is null || !User.Identity.IsAuthenticated) && CurrentAgencyId is null)
        {
            return Unauthorized();
        }

        return await base.DataTable(req);
    }

    protected override async Task<OneOf<Success, Error<string>>> BeforeCreateAsync(
        CreateFAQRequest createDto
    )
    {
        bool validAgencyUlid = Ulid.TryParse(createDto.AgencyId, out Ulid agencyUlid);
        if (!validAgencyUlid)
        {
            return new Error<string>("Invalid agency ulid");
        }

        var currentUser = (await _identityService.GetUser(User))!;

        var isAdmin = await _identityService.IsAdmin(currentUser);
        var IsAgent = await _identityService.IsAgent(currentUser);

        if (IsAgent && !isAdmin)
        {
            var userAgencyId = currentUser.AgencyId;
            if (userAgencyId is null)
            {
                return new Error<string>("Agent not authorized");
            }

            if (createDto.AgencyId != userAgencyId.ToString())
            {
                return new Error<string>("Agent not authorized agency ulid");
            }
        }

        if (!await _agencyRepository.ExistAsync(agencyUlid))
        {
            return new Error<string>("Invalid agency ulid");
        }

        return new Success();
    }

    // TODO: handle before update action with authorization

    protected override string[] GetSearchableProperties()
    {
        return new[] { nameof(FAQ.Question), nameof(FAQ.Answer) };
    }

    protected override string[] IncludeNavigation()
    {
        return new[] { nameof(FAQ.Agency) };
    }
}
