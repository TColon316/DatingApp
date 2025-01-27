using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController, Route("api/[controller]")] // /api/users
    public class UsersController(DataContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            var users = await context.Users.ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id:int}")] // /api/users/3
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUser(int id)
        {
            // Attempt to retrieve requested User
            var user = await context.Users.FindAsync(id);

            // If User is not found in the database, then return a NotFound exception
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}