using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;

    public UsersController(IMapper mapper, IUserRepository userRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers() {
    var users = await _userRepository.GetMembersAsync();

    return Ok(users);
  }

  [HttpGet("{id:int}")] // /api/users/1
  public async Task<ActionResult<AppUser>> GetUser(int id) {
    var user = await _userRepository.GetUserByIdAsync(id);

    if (user == null) return NotFound("User was not found");

    return Ok(user);
  }

  [HttpGet("{username}")]
  public async Task<ActionResult<MemberDto>> GetUser(string username)
  {
    var user = await _userRepository.GetMemberAsync(username);

    if (user == null) return NotFound("User was not found");

    return Ok(user);
  }

  [HttpPut]
  public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) {
    var userName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (userName == null) return BadRequest("No UserName found in token.");

    var user = await _userRepository.GetUserByNameAsync(userName);

    if (user == null) return BadRequest("Could not find User.");

    _mapper.Map(memberUpdateDto, user);

    if (await _userRepository.SaveAllAsync()) return NoContent();

    return BadRequest("Failed to update the user.");
  }
}
