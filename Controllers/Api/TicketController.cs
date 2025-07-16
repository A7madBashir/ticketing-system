using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;
using TicketingSystem.Models.DTO.Requests.Ticket;
using TicketingSystem.Models.DTO.Responses.Ticket;
using TicketingSystem.Models.Entities.Tickets;
using TicketingSystem.Models.Identity;
using TicketingSystem.Services;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

public class TicketController(
    ILogger<TicketController> logger,
    ITicketRepository repository,
    IAgencyRepository agencyRepository,
    ICategoryRepository categoryRepository,
    UserManager<User> userManager,
    IIdentityService identityService,
    Mapper mapper
)
    : CrudController<Ticket, Ulid, TicketResponse, CreateTicketRequest, EditTicketRequest>(
        repository,
        mapper
    )
{
    private readonly ILogger<TicketController> _logger = logger;

    private readonly IAgencyRepository _agencyRepo = agencyRepository;
    private readonly ICategoryRepository _categoryRepository = categoryRepository;
    private readonly UserManager<User> _userManager = userManager;
    private readonly IIdentityService _identityService = identityService;

    protected override async Task<OneOf<Success, Error<string>>> BeforeCreateAsync(
        CreateTicketRequest createDto
    )
    {
        // return Task.FromResult<OneOf<Success, Error<string>>>(new Success()); // Default: allow creation
        // check category id valid and exist also agency id
        bool validAgencyId = Ulid.TryParse(createDto.AgencyId, out Ulid AgencyUlid);
        if (!validAgencyId)
        {
            return new Error<string>("Invalid agency ulid");
        }

        if (!await _agencyRepo.ExistAsync(AgencyUlid))
        {
            return new Error<string>("Agency not exist");
        }

        bool validCategoryId = Ulid.TryParse(createDto.CategoryId, out Ulid CategoryUlid);
        if (!validCategoryId)
        {
            return new Error<string>("Invalid category ulid");
        }

        if (!await _categoryRepository.ExistAsync(CategoryUlid))
        {
            return new Error<string>("Category not exist");
        }

        createDto.CreatedById = (await _identityService.GetUser(User))!.Id.ToString();

        return new Success();
    }

    protected override string[] GetSearchableProperties()
    {
        return [nameof(Ticket.Title), nameof(Ticket.Description), nameof(Ticket.Status)];
    }

    protected override string[] IncludeNavigation()
    {
        return [nameof(Ticket.Category), nameof(Ticket.Agency), nameof(Ticket.CreatedBy)];
    }

    protected override async Task<OneOf<Success, Error<string>>> BeforeUpdateAsync(
        Ulid id,
        EditTicketRequest updateDto,
        Ticket existingEntity
    )
    {
        // Assign create by id user to dto model
        // Anther solution by adjusting entity model directly before update entity by using mapper of source and target as ref
        updateDto.CreatedById = existingEntity.CreatedById.ToString();

        return new Success();
    }
}
