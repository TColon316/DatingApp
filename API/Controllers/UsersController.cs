using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class UsersController : BaseApiController
{
    private readonly DataContext _context;

    public UsersController(DataContext context)
    {
        _context = context;
    }

  [AllowAnonymous]
  [HttpGet]
  public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() {
    var users = await _context.AppUsers.ToListAsync();

    return Ok(users);
  }

  [Authorize]
  [HttpGet("{id:int}")] // /api/users/1
  public async Task<ActionResult<AppUser>> GetUser(int id) {
    var user = await _context.AppUsers.FindAsync(id);

    if (user == null) return NotFound("User was not found");

    return Ok(user);
  }
}
