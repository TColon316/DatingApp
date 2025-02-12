using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(DataContext context, IMapper mapper, ITokenService tokenService) : BaseApiController
    {
        [HttpPost("register")] // account/register
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // Check if the UserName already exists in the database
            if (await UserExists(registerDto.Username)) return BadRequest("Username is already taken!");

            // We will use HMACSHA512 to encrypt text or use a hashing algorithm to encrypt some text
            using var hmac = new HMACSHA512();

            var user = mapper.Map<AppUser>(registerDto);

            user.UserName = registerDto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
            user.PasswordSalt = hmac.Key;

            // Add new user to the User table
            context.Users.Add(user);

            // Save changes to the database
            await context.SaveChangesAsync();

            // Return user
            return new UserDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Token = tokenService.CreateToken(user),
                Gender = user.Gender,
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // Look for existing user
            var user = await context.Users
                .Include(x => x.Photos)
                .FirstOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

            // If no user is found, then throw an Unauthorized exception
            if (user == null)
                return Unauthorized("Invalid username");

            // Compare the passwords in the database to the password that was provided
            using var hmac = new HMACSHA512(user.PasswordSalt);

            // Create a Computed Hash with the Password that was provided
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            // Loop over the array of bytes, comparing each element to ensure that they both match (meaning the user entered a correct password)
            for (int i = 0; i < computedHash.Length; i++)
            {
                // If at any point the hashes do not match, it means that the password is incorrect. Throw an Unauthorized exception.
                if (computedHash[i] != user.PasswordHash[i])
                    return Unauthorized("Invalid Password");
            }

            // If this line of code is reached, then it means the login information was valid
            return new UserDto
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Token = tokenService.CreateToken(user),
                Gender = user.Gender,
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
            };
        }

        // Method to check if the UserName already exists in the database
        private async Task<bool> UserExists(string username)
        {
            return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }
    }
}