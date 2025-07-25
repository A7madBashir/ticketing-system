using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using TicketingSystem.Handlers.DataTable;
using TicketingSystem.Models.Common;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.DTO.Requests;
using TicketingSystem.Models.DTO.Responses;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

// Generic CRUD Controller
// TEntity: The database entity type (must implement IEntity<Ulid>)
// TCreateRequest: The DTO for create requests (must inherit BaseCreateDto)
// TUpdateRequest: The DTO for update requests (must inherit BaseUpdateDto)
public abstract class CrudController<TEntity, T, TResponse, TCreateRequest, TUpdateRequest>
    : ApiBaseController
    where TEntity : class, IEntity<T> // TEntity must be a class and have a ULID PK
    where T : IEquatable<T>
    where TCreateRequest : ICreateRequest
    where TUpdateRequest : IEditRequest<T>
    where TResponse : BaseResponse
{
    protected readonly IRepository<TEntity, T> _repository;
    private readonly Mapper _mapper;
    private readonly DataTableHandler<TEntity, T> _dataTable;
    protected Ulid? CurrentAgencyId => (Ulid)HttpContext.Items["AgencyId"]!;

    // Constructor injection
    public CrudController(IRepository<TEntity, T> repository, Mapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
        _dataTable = new DataTableHandler<TEntity, T>(repository, mapper);
    }

    /// <summary>
    /// Derived classes MUST implement this to tell the DataTableService what properties to search within
    /// for the general search term.
    /// </summary>
    protected virtual string[]? GetSearchableProperties()
    {
        return null;
    }

    /// <summary>
    /// Specify navigation models
    /// </summary>
    protected virtual string[]? IncludeNavigation()
    {
        return null;
    }

    /// <summary>
    /// Derived classes can override this to provide a default ordering expression for their data table.
    /// If not overridden, it defaults to ordering by 'Id'.
    /// </summary>
    protected virtual Expression<Func<TEntity, object>> GetDefaultOrderBy()
    {
        // Default to ordering by 'Id' if no specific order is provided
        return e => e.Id;
    }

    /// <summary>
    /// Derived classes can override this to apply a default or global filter to their table data.
    /// This filter is applied *before* any other searching, filtering, or pagination.
    /// </summary>
    protected virtual Expression<Func<TEntity, bool>>? GetInitialWhereClause()
    {
        return null; // Default to no initial filter
    }

    /// <summary>
    /// Custom override point to build the base query before any pagination, sorting, or general searching.
    /// This is where derived classes can include navigation properties, apply advanced filters,
    /// or change the starting point of the query.
    /// </summary>
    /// <param name="initialQuery">The initial IQueryable from the repository.</param>
    /// <returns>The IQueryable after applying custom includes/filters.</returns>
    protected virtual async Task<IQueryable<TEntity>?> BuildBaseQuery(DataTableRequest req)
    {
        // By default, do nothing and return the initial query.
        // Derived classes will override this to add .Include(), .Where(), etc.
        return null;
    }

    // --- Hooks / Event Methods (Virtual, to be overridden by derived controllers) ---

    /// <summary>
    /// Hook to execute custom logic BEFORE a create operation.
    /// Can modify the create DTO or return an error to prevent creation.
    /// </summary>
    /// <param name="createDto">The DTO received from the client for creation.</param>
    /// <returns>Success if creation should proceed, Error with message if it should be prevented.</returns>
    protected virtual Task<OneOf<Success, Error<string>>> BeforeCreateAsync(
        TCreateRequest createDto
    )
    {
        return Task.FromResult<OneOf<Success, Error<string>>>(new Success()); // Default: allow creation
    }

    /// <summary>
    /// Hook to execute custom logic BEFORE a create entity and after convert dto to entity using mapper operation.
    /// Can modify the create DTO or return an error to prevent creation.
    /// </summary>
    /// <param name="entity">The Entity model after mapped from dto client model.</param>
    /// <returns>Success if creation should proceed, Error with message if it should be prevented.</returns>
    protected virtual Task<OneOf<Success, Error<string>>> BeforeCreateEntityAsync(TEntity entity)
    {
        return Task.FromResult<OneOf<Success, Error<string>>>(new Success()); // Default: allow creation
    }

    /// <summary>
    /// Hook to execute custom logic AFTER a create operation has successfully saved to DB.
    /// </summary>
    /// <param name="createdEntity">The entity that was just created and saved.</param>
    protected virtual Task AfterCreateAsync(TEntity createdEntity)
    {
        return Task.CompletedTask; // Default: do nothing
    }

    /// <summary>
    /// Hook to execute custom logic BEFORE an update operation.
    /// Can modify the update DTO, entity, or return an error to prevent update.
    /// </summary>
    /// <param name="id">The ID of the entity to update.</param>
    /// <param name="updateDto">The DTO received from the client for update.</param>
    /// <param name="existingEntity">The entity retrieved from the database.</param>
    /// <returns>Success if update should proceed, Error with message if it should be prevented.</returns>
    protected virtual Task<OneOf<Success, Error<string>>> BeforeUpdateAsync(
        T id,
        TUpdateRequest updateDto,
        TEntity existingEntity
    )
    {
        return Task.FromResult<OneOf<Success, Error<string>>>(new Success()); // Default: allow update
    }

    /// <summary>
    /// Hook to execute custom logic BEFORE a create entity and after convert dto to entity using mapper operation.
    /// Can modify the create DTO or return an error to prevent creation.
    /// </summary>
    /// <param name="entity">The Entity model after mapped from dto client model.</param>
    /// <returns>Success if creation should proceed, Error with message if it should be prevented.</returns>
    protected virtual Task<OneOf<Success, Error<string>>> BeforeUpdateEntityAsync(TEntity entity,TEntity oldEntity)
    {
        return Task.FromResult<OneOf<Success, Error<string>>>(new Success()); // Default: allow creation
    }

    /// <summary>
    /// Hook to execute custom logic AFTER an update operation has successfully saved to DB.
    /// </summary>
    /// <param name="updatedEntity">The entity that was just updated and saved.</param>
    protected virtual Task AfterUpdateAsync(TEntity updatedEntity)
    {
        return Task.CompletedTask; // Default: do nothing
    }

    /// <summary>
    /// Hook to execute custom logic BEFORE a delete operation.
    /// Can return an error to prevent deletion.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <param name="entityToDelete">The entity retrieved from the database.</param>
    /// <returns>Success if deletion should proceed, Error with message if it should be prevented.</returns>
    protected virtual Task<OneOf<Success, Error<string>>> BeforeDeleteAsync(
        T id,
        TEntity entityToDelete
    )
    {
        return Task.FromResult<OneOf<Success, Error<string>>>(new Success()); // Default: allow deletion
    }

    /// <summary>
    /// Hook to execute custom logic AFTER a delete operation has successfully removed from DB.
    /// </summary>
    /// <param name="deletedEntity">The entity that was just deleted.</param>
    protected virtual Task AfterDeleteAsync(TEntity deletedEntity)
    {
        return Task.CompletedTask; // Default: do nothing
    }

    // --- CRUD Endpoints ---

    [HttpGet]
    public virtual async Task<ActionResult<PaginatedResponse<TResponse>>> DataTable(
        [FromQuery] DataTableRequest req
    )
    {
        if (req.Page < 1)
        {
            return BadRequest("Page number not valid");
        }

        if (req.Count < -1)
        {
            return BadRequest("Count less than zero");
        }

        if (string.IsNullOrEmpty(req.Search))
        {
            req.Search = Request.Query["search[value]"].ToString();
        }

        var res = await _dataTable.PaginatedDataAsync<TResponse>(
            req.Page,
            req.Count,
            req.Draw,
            await BuildBaseQuery(req),
            req.Search,
            GetSearchableProperties(),
            GetInitialWhereClause(),
            GetDefaultOrderBy(),
            orderByDescending: req.OrderByDescending,
            includeNavigations: IncludeNavigation(),
            selector: _mapper.ToResponse<TResponse, T>
        );

        return Ok(res);
    }

    // GET: api/[controller]/{id}
    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TResponse>> Get(T id)
    {
        var entity = await _repository.GetByIdAsync(id, IncludeNavigation());
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(_mapper.ToResponse<TResponse, T>(entity)); // Map entity to DTO
    }

    // POST: api/[controller]
    [HttpPost]
    public virtual async Task<ActionResult<TResponse>> Create([FromBody] TCreateRequest createDto)
    {
        if (!ModelState.IsValid)
        {
            // TODO: Error model response handling
            return BadRequest(ModelState);
        }

        // Execute BeforeCreate hook
        var beforeCreateResult = await BeforeCreateAsync(createDto);
        if (beforeCreateResult.IsT1)
        {
            return BadRequest(beforeCreateResult.Value); // Return error from hook
        }

        var entity = _mapper.ToEntity<TEntity, T>(createDto); // Map DTO to entity
        // Execute BeforeCreate hook
        var beforeCreateEntity = await BeforeCreateEntityAsync(entity);
        if (beforeCreateEntity.IsT1)
        {
            return BadRequest(beforeCreateEntity.Value); // Return error from hook
        }

        await _repository.AddAsync(entity);

        // Execute AfterCreate hook
        await AfterCreateAsync(entity);

        return CreatedAtAction(
            nameof(Get),
            new { id = entity.Id },
            _mapper.ToResponse<TResponse, T>(entity)
        );
    }

    // PUT: api/[controller]/{id}
    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Update(string id, [FromBody] TUpdateRequest updateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(
                ErrorResponse.BadRequest(HttpContext, ModelState, ErrorCodes.InvalidModelState)
            );
        }

        // check if T type of Ulid or int for parsing
        var isIntegerId = typeof(T) == typeof(int);
        if (isIntegerId)
        {
            if (!int.TryParse(id, out int intId))
                return BadRequest(ErrorResponse.BadRequest(HttpContext, ModelState, "Invalid id"));
            else
                updateDto.Id = (dynamic)intId;
        }
        else
        {
            bool validUlid = Ulid.TryParse(id, out Ulid ulid);
            if (!validUlid)
            {
                return BadRequest(
                    ErrorResponse.BadRequest(HttpContext, ModelState, ErrorCodes.InvalidUlid)
                );
            }

            updateDto.Id = (dynamic)ulid;
        }

        var existingEntity = await _repository
            .Query() // using query for no tracking purpose
            .FirstOrDefaultAsync(e => e.Id.Equals(updateDto.Id));
        if (existingEntity == null)
        {
            return NotFound();
        }

        // Execute BeforeUpdate hook
        var beforeUpdateResult = await BeforeUpdateAsync(updateDto.Id, updateDto, existingEntity);
        if (beforeUpdateResult.IsT1)
        {
            return BadRequest(beforeUpdateResult.Value);
        }

        // Map DTO to existing entity (important to update the existing object)
        var entity = _mapper.ToEntity<TEntity, T>(updateDto);

        // Execute BeforeCreate hook
        var beforeUpdateEntity = await BeforeUpdateEntityAsync(entity,existingEntity);
        if (beforeUpdateEntity.IsT1)
        {
            return BadRequest(beforeUpdateEntity.Value); // Return error from hook
        }

        await _repository.UpdateAsync(entity);

        // Execute AfterUpdate hook
        await AfterUpdateAsync(entity);

        return Ok(); // 200
    }

    // DELETE: api/[controller]/{id}
    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(string id)
    {
        T dbId;
        // check if T type of Ulid or int for parsing
        var isIntegerId = typeof(T) == typeof(int);
        if (isIntegerId)
        {
            if (!int.TryParse(id, out int intId))
                return BadRequest(ErrorResponse.BadRequest(HttpContext, ModelState, "Invalid id"));
            else
                dbId = (dynamic)intId;
        }
        else
        {
            bool validUlid = Ulid.TryParse(id, out Ulid ulid);
            if (!validUlid)
            {
                return BadRequest(
                    ErrorResponse.BadRequest(HttpContext, ModelState, ErrorCodes.InvalidUlid)
                );
            }

            dbId = (dynamic)ulid;
        }
        var entityToDelete = await _repository.GetByIdAsync(dbId);
        if (entityToDelete == null)
        {
            return NotFound();
        }

        // Execute BeforeDelete hook
        var beforeDeleteResult = await BeforeDeleteAsync(dbId, entityToDelete);
        if (beforeDeleteResult.IsT1)
        {
            return BadRequest(beforeDeleteResult.Value);
        }

        await _repository.DeleteAsync(dbId);

        // Execute AfterDelete hook
        await AfterDeleteAsync(entityToDelete);

        return Ok();
    }
}
