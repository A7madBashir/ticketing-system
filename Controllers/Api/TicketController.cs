using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OneOf;
using OneOf.Types;
using TicketingSystem.Filters;
using TicketingSystem.Models.Common;
using TicketingSystem.Models.DTO.Requests;
using TicketingSystem.Models.DTO.Requests.Ticket;
using TicketingSystem.Models.DTO.Responses.Ticket;
using TicketingSystem.Models.Entities.Tickets;
using TicketingSystem.Models.Identity;
using TicketingSystem.Services;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

[Authorize(Policy = AuthenticationPolicy.AgentAccess)]
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
    private readonly Mapper _mapper = mapper;

    protected override async Task<IQueryable<Ticket>>? BuildBaseQuery(DataTableRequest req)
    {
        // Get the base query from the repository (which typically returns an AsNoTracking query)
        var query = _repository.Query(
            [nameof(Ticket.Agency), nameof(Ticket.CreatedBy), nameof(Ticket.Category)]
        );

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

        return query;
    }

    public override async Task<ActionResult<TicketResponse>> Create(
        [FromBody] CreateTicketRequest createDto
    )
    {
        // is admin
        var isAdmin = await _identityService.IsAdmin(User);
        if (!isAdmin)
        {
            var currentUser = (await _identityService.GetUser(User))!;
            if (currentUser.AgencyId is null)
            {
                return Unauthorized();
            }

            createDto.AgencyId = currentUser.AgencyId.ToString()!;
        }

        return await base.Create(createDto);
    }

    protected override Expression<Func<Ticket, object>> GetDefaultOrderBy()
    {
        return e => e.CreateTime;
    }

    [AllowAnonymous]
    [ApiKeyAuthorize]
    [HttpPost("AgencyTicket")]
    public async Task<IActionResult> AnonymousTicket([FromBody] CreateAnonymousTicket request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(
                ErrorResponse.OnlyMessage(
                    HttpContext,
                    ModelState,
                    ErrorCodes.InvalidModelState,
                    "Please try again"
                )
            );
        }

        // generate agency new ticket
        var user = await _identityService.GetOrAddUser(
            request.Email,
            request.PhoneNumber,
            request.Name
        );
        // validate fields
        // check user existent or not
        // handle new user
        if (user is null)
        {
            return BadRequest(
                ErrorResponse.OnlyMessage(
                    HttpContext,
                    ModelState,
                    "Failed to add user",
                    "Please try again later"
                )
            );
        }

        if (CurrentAgencyId is null)
        {
            return BadRequest(
                ErrorResponse.OnlyMessage(
                    HttpContext,
                    ModelState,
                    "Failed to get agency",
                    "Please try again later"
                )
            );
        }

        user.AgencyId = CurrentAgencyId;
        await _userManager.UpdateAsync(user);
        var userResponse = _mapper.ToUserProfile(user);

        var ticket = new Ticket
        {
            Title = userResponse.Name + ": " + "new issue",
            Description = request.Content,
            AgencyId = (Ulid)CurrentAgencyId,
            Status = "Open",
            Priority = "Low",
            CreatedById = user.Id,
            OriginatedFromChatbot = true,
        };

        await _repository.AddAsync(ticket);
        // handle new ticket with default values

        return Ok();
    }

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
