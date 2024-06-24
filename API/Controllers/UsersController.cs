using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API;

[ApiController]
[Route("api/[controller]")] // /api/users
public class UsersController : ControllerBase
{
    private readonly DataContext _context;

    public UsersController(DataContext context)
  {
        _context = context;
    }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers() {
    var users = await _context.AppUsers.ToListAsync();

    return Ok(users);
  }

  [HttpGet("{id:int}")] // /api/users/1
  public async Task<ActionResult<AppUser>> GetUser(int id) {
    var user = await _context.AppUsers.FindAsync(id);

    if (user == null) return NotFound("User was not found");

    return Ok(user);
  }
}
