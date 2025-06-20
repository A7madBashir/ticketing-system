using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using TicketingSystem.Models.Common;
using TicketingSystem.Models.DTO.Requests.User;
using TicketingSystem.Models.DTO.Responses.User;
using TicketingSystem.Models.Identity;
using TicketingSystem.Services.Repositories;

namespace TicketingSystem.Controllers.Api;

public class UserController(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IUserRepository repository,
    Mapper mapper
) : CrudController<User, Ulid, UserResponse, RegisterUser, UpdateUserRequest>(repository, mapper)
{
    private readonly UserManager<User> _userManager = userManager;

    private readonly IUserRepository _repository = repository;
    private readonly Mapper _mapper = mapper;

    // [HttpPost("register")]
    // [AllowAnonymous] // Allow anyone to register
    // public async Task<IActionResult> Register([FromBody] RegisterUser request)
    // {
    //     if (!ModelState.IsValid)
    //     {
    //         return BadRequest(
    //             ErrorResponse.BadRequest(HttpContext, ModelState, ErrorCodes.InvalidModelState)
    //         );
    //     }

    //     User user = _mapper.ToEntity(request);
    //     user.UserName = user.Email ?? user.PhoneNumber ?? user.FirstName + user.LastName;

    //     var result = await _userManager.CreateAsync(user);
    //     if (result.Succeeded)
    //     {
    //         await _userManager.AddToRoleAsync(user, Roles.User);
    //     }
    //     else
    //     {
    //         return BadRequest(
    //             ErrorResponse.BadRequest(
    //                 HttpContext,
    //                 ModelState,
    //                 "Failed to create new user, please try again!"
    //             )
    //         );
    //     }

    //     if (!string.IsNullOrEmpty(request.Password))
    //     {
    //         await _userManager.AddPasswordAsync(user, request.Password);

    //         // TODO: add validation error for password
    //     }

    //     return Ok(_mapper.ToUserProfile(user));
    // }

    [HttpGet("profile")]
    public async Task<ActionResult<Profile>> GetCurrentUserProfile()
    {
        var username = User.Identity.Name ?? "";
        var userId = User.FindFirst(c => c.Type == JwtRegisteredClaimNames.Jti)?.Value;

        bool validUlid = Ulid.TryParse(userId, out Ulid userUlid);
        if (!string.IsNullOrEmpty(userId) && !validUlid)
        {
            return BadRequest(
                ErrorResponse.BadRequest(HttpContext, ModelState, ErrorCodes.InvalidUlid)
            );
        }

        User user;
        if (!string.IsNullOrEmpty(userId))
        {
            user = await _repository.GetByIdAsync(userUlid);
        }
        else
        {
            user = await _repository.FirstOrDefaultAsync(u => u.UserName == username);
        }

        if (user == null)
        {
            return NotFound("User not found.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var profile = _mapper.ToUserProfile(user);
        if (roles is not null && roles.Count > 0)
            profile.Roles = [.. roles];

        return Ok(profile);
    }

    [HttpPut("profile")]
    // [Authorize] is already on the controller level
    public async Task<IActionResult> UpdateCurrentUserProfile([FromBody] UpdateUserRequest request)
    {
        // TODO:
        // 1_ validate inputs
        // 2_ validate user exist with ulid is valid
        // 3_ update user model
        // 4_ handle error if occur

        return Ok();
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        // TODO:
        // 1_ validate inputs
        // 2_ validate user exist with ulid is valid
        // 3_ update user password
        // 4_ handle error if occur
        // ! if current user is admin or owner can then modify the password
        return Ok();
    }

    // [HttpDelete("{id}")]
    // [Authorize(Roles = Roles.Admin)]
    // public async Task<IActionResult> DeleteUser(string id)
    // {
    //     var user = await _userManager.FindByIdAsync(id);
    //     if (user == null)
    //     {
    //         return NotFound("User not found.");
    //     }

    //     var result = await _userManager.DeleteAsync(user);

    //     if (result.Succeeded)
    //     {
    //         return NoContent(); // 204 No Content
    //     }

    //     return BadRequest(
    //         ErrorResponse.OnlyMessage(
    //             null,
    //             ModelState,
    //             "Delete user failed",
    //             "Failed to delete user please try again!"
    //         )
    //     );
    // }
}
