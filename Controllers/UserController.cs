using Microsoft.AspNetCore.Mvc;
using Riok.Mapperly.Abstractions;
using TicketingSystem.Mapper;
using TicketingSystem.Models.DTO.Requests.User;
using TicketingSystem.Models.DTO.Responses.User;
using TicketingSystem.Models.Identity;
using Ticketingsystem.Repositories;

namespace TicketingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IRepository<User> _userRepository;
    private readonly UserMapper _userMapper;

    public UserController(IRepository<User> userRepository, UserMapper userMapper)
    {
        _userRepository = userRepository;
        _userMapper = userMapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users.Select(_userMapper.ToResponseDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponseDto>> GetById(Ulid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound();

        return Ok(_userMapper.ToResponseDto(user));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create(UserRequestDto request)
    {
        var user = _userMapper.ToEntity(request);
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            _userMapper.ToResponseDto(user)
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Ulid id, UserRequestDto request)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        var updatedUser = _userMapper.ToEntity(request);
        updatedUser.Id = id;

        _userRepository.Update(updatedUser);
        await _userRepository.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Ulid id)
    {
        var existing = await _userRepository.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        _userRepository.Delete(existing);
        await _userRepository.SaveChangesAsync();

        return NoContent();
    }
}
