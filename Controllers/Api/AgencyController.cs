using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TicketingSystem.Filters;
using TicketingSystem.Models.Common;
using TicketingSystem.Models.DTO.Requests;
using TicketingSystem.Models.DTO.Requests.Agency;
using TicketingSystem.Models.DTO.Responses.Agency;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Services;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

[Authorize(Policy = AuthenticationPolicy.AdminOnly)]
// AgencyController inherits from CrudController, specializing it for Agency entities.
// It uses Ulid as the primary key type.
public class AgencyController(
    IAgencyRepository repository,
    IIdentityService identityService,
    Mapper mapper
) : CrudController<Agency, Ulid, AgencyResponse, CreateAgency, EditAgency>(repository, mapper)
{
    private readonly IIdentityService _identityService = identityService;

    // Private fields to hold injected dependencies (can also use protected fields from base if available)
    private readonly IAgencyRepository _agencyRepository = repository;
    private readonly Mapper _mapper = mapper; // Still needed for DataTable selector and potentially other mappings

    /// <summary>
    /// Overrides the base method to specify which properties of the Agency entity
    /// should be considered when a general search term is applied to the DataTable.
    /// This is still relevant for read operations (DataTable).
    /// </summary>
    /// <returns>An array of property names that are searchable.</returns>
    protected override string[]? GetSearchableProperties()
    {
        // Allows searching by Agency Name and Domain for the DataTable's global search box.
        return new[] { nameof(Agency.Name), nameof(Agency.Domain) };
    }

    /// <summary>
    /// Overrides the base method to build the initial IQueryable for fetching Agency data.
    /// This is crucial for eager loading related entities like Subscription for both
    /// single Get operations and the DataTable endpoint.
    /// </summary>
    /// <returns>An IQueryable<Agency> with necessary includes applied.</returns>
    protected override async Task<IQueryable<Agency>?> BuildBaseQuery(DataTableRequest req)
    {
        // Get the base query from the repository (which typically returns an AsNoTracking query)
        var query = _agencyRepository.Query();

        // Eager load the 'Subscription' navigation property.
        // This ensures that when Agency data is fetched, its associated Subscription data is also loaded.
        query = query.Include(a => a.Subscription);
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
                query = query.Where(q => q.Id == agencyUlid);
            }
        }

        if (req.Filters.Count > 0)
        {
            // Filter by subscription plan id
            string subscriptionId = req
                .Filters.Where(r => r.Key == "subscriptionId")
                .FirstOrDefault()
                .Value;
            if (
                !string.IsNullOrEmpty(subscriptionId)
                && Ulid.TryParse(subscriptionId, out Ulid subId)
            )
            {
                query = query.Where(r => r.SubscriptionId == subId);
            }
        }

        return query;
    }

    /// <summary>
    /// Overrides the base method to define the default sorting order for the Agency DataTable.
    /// This is still relevant for read operations (DataTable).
    /// </summary>
    /// <returns>An expression specifying the property to sort by.</returns>
    protected override Expression<Func<Agency, object>> GetDefaultOrderBy()
    {
        // Default order agencies by their Name.
        return a => a.Name;
    }

    /// <summary>
    /// Overrides the base method to specify navigation properties to include
    /// when fetching a single entity (e.g., for the Edit modal).
    /// </summary>
    protected override string[]? IncludeNavigation()
    {
        // Include the 'Subscription' navigation property when fetching a single Agency.
        return new[] { nameof(Agency.Subscription) };
    }

    // The 'Create' method override is removed. The base CrudController's virtual Create method will be used.
    // The 'Update' method override is removed. The base CrudController's virtual Update method will be used.
    // The 'Get' (for single record by ID) and 'Delete' methods are handled by the base CrudController.
    // The 'DataTable' (for paginated list) method is also handled by the base CrudController.
    // All these base methods will utilize the overridden GetSearchableProperties, BuildBaseQuery,
    // GetDefaultOrderBy, and now IncludeNavigation methods from this AgencyController for customization.
}
