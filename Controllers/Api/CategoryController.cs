using OneOf;
using OneOf.Types;
using TicketingSystem.Models.DTO.Requests.Category;
using TicketingSystem.Models.DTO.Responses.Category;
using TicketingSystem.Models.Entities.Agency;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

public class CategoryController(
    ICategoryRepository repository,
    IAgencyRepository agencyRepository,
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
    private readonly IAgencyRepository _agencyRepository = agencyRepository;

    protected override async Task<OneOf<Success, Error<string>>> BeforeCreateAsync(
        CreateCategoryRequest createDto
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
}
