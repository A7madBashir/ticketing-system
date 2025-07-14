using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using TicketingSystem.Models.DTO.Requests;
using TicketingSystem.Models.DTO.Requests.FAQ;
using TicketingSystem.Models.DTO.Responses.FAQ;
using TicketingSystem.Models.FAQs;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

public class FAQController(
    IFAQRepository repository,
    IAgencyRepository agencyRepository,
    Mapper mapper
)
    : CrudController<
        FAQ,
        Ulid,
        FAQResponseDto,
        CreateFAQRequest,
        EditFAQRequest
    >(repository, mapper)
{
    private readonly IFAQRepository _repository = repository;
    private readonly IAgencyRepository _agencyRepository = agencyRepository;

    protected override IQueryable<FAQ>? BuildBaseQuery(DataTableRequest req)
    {
        // Get the base query from the repository (which typically returns an AsNoTracking query)
        var query = _repository.Query();

        // This ensures that when Agency data is fetched, its associated Subscription data is also loaded.
        query = query.Include(a => a.Agency);

        if (req.Filters.Count > 0)
        {
            // Filter by subscription plan id
            string agencyId = req
                .Filters.Where(r => r.Key == "agencyId")
                .FirstOrDefault()
                .Value;
            if (
                !string.IsNullOrEmpty(agencyId)
                && Ulid.TryParse(agencyId, out Ulid subId)
            )
            {
                query = query.Where(r => r.AgencyId == subId);
            }
        }

        return query;
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

        if (!await _agencyRepository.ExistAsync(agencyUlid))
        {
            return new Error<string>("Invalid agency ulid");
        }

        return new Success();
    }

    protected override string[] GetSearchableProperties()
    {
       return new[] { nameof(FAQ.Question), nameof(FAQ.Answer) };
    }

    protected override string[] IncludeNavigation()
    {
       return new[] { nameof(FAQ.Agency) };
    }
   
}
