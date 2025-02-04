using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController(IUserRepository userRepository) : BaseApiController
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
    }
}