using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using TicketingSystem.Helper;
using TicketingSystem.Models.Common;
using TicketingSystem.Models.DTO.Requests;
using TicketingSystem.Models.DTO.Requests.Integrations;
using TicketingSystem.Models.DTO.Responses.Integrations;
using TicketingSystem.Models.Integrations;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

[Authorize(Policy = AuthenticationPolicy.AdminOnly)]
public class IntegrationController(
    IIntegrationsRepository repository,
    IAgencyRepository agencyRepository,
    Mapper mapper
)
    : CrudController<
        Integration,
        Ulid,
        IntegrationResponse,
        CreateIntegrationRequest,
        UpdateIntegrationRequest
    >(repository, mapper)
{
    // C# 12 Primary Constructors make these redundant.
    // The constructor parameters (repository, agencyRepository, mapper)
    // are directly accessible as fields within the class methods.
    // Keeping them explicitly assigned to private fields is generally not needed
    // unless you have a specific reason (e.g., passing them to a base class that doesn't
    // support primary constructors directly, which is not the case with your CrudController).
    // private readonly IIntegrationRepository _repository = repository; // REMOVE OR COMMENT OUT
    // private readonly IAgencyRepository _agencyRepository = agencyRepository; // REMOVE OR COMMENT OUT

    protected override async Task<IQueryable<Integration>?> BuildBaseQuery(DataTableRequest req)
    {
        // Use 'repository' directly from the primary constructor
        var query = repository.Query();

        // Include the Agency navigation property
        query = query.Include(a => a.Agency);

        if (req.Filters.Count > 0)
        {
            // Safer way to extract filter value from Dictionary<string, string>
            // req.Filters.FirstOrDefault(r => r.Key == "agencyId").Value can lead to NullReferenceException
            // if "agencyId" key is not found, as FirstOrDefault on a struct returns default struct (empty KeyValuePair).
            string? agencyIdFilterValue = req
                .Filters.FirstOrDefault(f => f.Key == "agencyId")
                .Value;

            if (
                !string.IsNullOrEmpty(agencyIdFilterValue) // Check if the value is actually there
                && Ulid.TryParse(agencyIdFilterValue, out Ulid parsedAgencyUlid) // Parse it safely
            )
            {
                // Correctly filter by the parsed Ulid
                query = query.Where(r => r.AgencyId == parsedAgencyUlid);
            }
        }

        return query;
    }

    protected override async Task<OneOf<Success, Error<string>>> BeforeCreateAsync(
        CreateIntegrationRequest createDto
    )
    {
        // Assuming createDto.AgencyId is already a Ulid, as that's typical for IDs in DTOs.
        // If createDto.AgencyId is a string, then the Ulid.TryParse logic below is correct.
        // Let's assume createDto.AgencyId is string based on your original code's parsing.

        Ulid agencyUlid;
        // Correctly parse createDto.AgencyId (assuming it's a string in the DTO)
        if (!Ulid.TryParse(createDto.AgencyId, out agencyUlid))
        {
            return new Error<string>("Invalid agency ID format provided."); // More specific error message
        }

        // Use 'agencyRepository' directly from the primary constructor
        if (!await agencyRepository.ExistAsync(agencyUlid))
        {
            return new Error<string>("Agency with the provided ID does not exist."); // More specific error message
        }

        return new Success();
    }

    protected override async Task<OneOf<Success, Error<string>>> BeforeCreateEntityAsync(Integration entity)
    {
        entity.ApiKey = ApiKeyGenerator.Generate();

        return new Success();
    }

    protected override async Task<OneOf<Success, Error<string>>> BeforeUpdateEntityAsync(
        Integration entity,
        Integration oldEntity
    )
    {
        entity.ApiKey = oldEntity.ApiKey;
        return new Success();
    }

    protected override string[] GetSearchableProperties()
    {
        // These properties (Question, Answer) likely refer to properties on the Integration entity itself.
        // If you want to search on Agency properties, ensure Agency is included and add:
        // nameof(Integration.Agency.Name) // Example if Agency has a Name property
        return new[] { nameof(Integration.Name), nameof(Integration.Enabled) };
    }

    protected override string[] IncludeNavigation()
    {
        // This array should contain the names of navigation properties that the base CrudController
        // should include when fetching entities (e.g., for single Get by ID).
        // It's consistent with what you're doing in BuildBaseQuery.
        return new[] { nameof(Integration.Agency) };
    }
}
