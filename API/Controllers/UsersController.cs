using System.Security.Claims;
using API.DTOs;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(IMapper mapper, IUserRepository userRepository) : BaseApiController
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
        {
            var users = await userRepository.GetMembersAsync();

            return Ok(users);
        }

        [HttpGet("{username}")] // /api/users/3
        public async Task<ActionResult<MemberDto>> GetUser(string username)
        {
            // Attempt to retrieve requested User
            var user = await userRepository.GetMemberAsync(username);

            // If User is not found in the database, then return a NotFound exception
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            // Get the UserName of the currently logged in User from the ClaimsPrincipal
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if username is empty and return an exception if so
            if (username == null)
                return BadRequest("No username found in token");

            // Pass username parameter to see if there is a user in the system
            var user = await userRepository.GetUserByNameAsync(username);

            // If no User is found, then return an exception indicating this
            if (user == null)
                return BadRequest("Could not find user");

            // This will transfer the changes made by the user (Parameter memberUpdateDto) to the user record pulled from the DB
            mapper.Map(memberUpdateDto, user);

            // Save all changes
            if (await userRepository.SaveAllAsync())
                return NoContent(); // This is the correct response for a successful HttpPut request

            // If save is unsuccessful then return a BadRequest
            return BadRequest("Failed to update the user");
        }
    }
}