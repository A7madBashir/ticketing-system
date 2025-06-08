using Microsoft.AspNetCore.Identity.UI.V4.Pages.Internal;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using TicketingSystem.Models.Common;
using TicketingSystem.Models.Common.BaseEntity;
using TicketingSystem.Models.DTO.Requests;
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
{
    protected readonly IRepository<TEntity, T> _repository;
    private readonly Mapper _mapper;

    // Constructor injection
    public CrudController(IRepository<TEntity, T> repository, Mapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
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

    // GET: api/[controller]/{id}
    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TResponse>> Get(T id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(_mapper.ToResponse<TResponse, T>(entity)); // Map entity to DTO
    }

    // GET: api/[controller]
    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TResponse>>> GetAll()
    {
        var entities = (await _repository.GetAllAsync()).Select(_mapper.ToResponse<TResponse, T>);
        return Ok(entities); // Map entities to DTOs
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

        var existingEntity = await _repository.GetByIdAsync(updateDto.Id);
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
        var item = _mapper.ToEntity<TEntity, T>(updateDto);
        await _repository.UpdateAsync(item);

        // Execute AfterUpdate hook
        await AfterUpdateAsync(item);

        return NoContent(); // 204 No Content
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

        return NoContent(); // 204 No Content
    }
}
