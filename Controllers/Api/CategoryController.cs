using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using TicketingSystem.Models.DTO.Requests;
using TicketingSystem.Models.DTO.Requests.Category;
using TicketingSystem.Models.DTO.Responses.Category;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Services;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

public class CategoryController(
    ICategoryRepository repository,
    IAgencyRepository agencyRepository,
    IIdentityService identityService,
    Mapper mapper
)
    : CrudController<
        Category,
        Ulid,
        CategoryResponseDto,
        CreateCategoryRequest,
        EditCategoryRequest
    >(repository, mapper)
{
    private readonly ICategoryRepository _repository = repository;
    private readonly IAgencyRepository _agencyRepository = agencyRepository;
    private readonly IIdentityService _identityService = identityService;

    /// <summary>
    /// Overrides the base method to build the initial IQueryable for fetching Agency data.
    /// This is crucial for eager loading related entities like Subscription for both
    /// single Get operations and the DataTable endpoint.
    /// </summary>
    /// <returns>An IQueryable<Agency> with necessary includes applied.</returns>
    protected override async Task<IQueryable<Category>?> BuildBaseQuery(DataTableRequest req)
    {
        // Get the base query from the repository (which typically returns an AsNoTracking query)
        var query = _repository.Query();

        // This ensures that when Agency data is fetched, its associated Subscription data is also loaded.
        query = query.Include(a => a.Agency);

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

        if (req.Filters.Count > 0)
        {
            // Filter by subscription plan id
            string agencyId = req.Filters.Where(r => r.Key == "agencyId").FirstOrDefault().Value;
            if (!string.IsNullOrEmpty(agencyId) && Ulid.TryParse(agencyId, out Ulid subId))
            {
                query = query.Where(r => r.AgencyId == subId);
            }
        }

        return query;
    }

    protected override async Task<OneOf<Success, Error<string>>> BeforeCreateAsync(
        CreateCategoryRequest createDto
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

    protected override string[] GetSearchableProperties()
    {
        return new[] { nameof(Category.Name), nameof(Category.Description) };
    }

    protected override string[] IncludeNavigation()
    {
        return new[] { nameof(Category.Agency) };
    }
}
