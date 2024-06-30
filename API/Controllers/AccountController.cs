using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{
  private readonly DataContext _context;
  private readonly ITokenService _tokenService;

  public AccountController(DataContext context, ITokenService tokenService)
  {
      _tokenService = tokenService;
      _context = context;
  }

  [HttpPost("register")]
  public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
  {
    // Check if user already exists in the User table
    if (await UserExists(registerDto.UserName)) return BadRequest("Username is already taken.");

    using var hmac = new HMACSHA512();

    var user = new AppUser
    {
      UserName = registerDto.UserName,
      PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
      PasswordSalt = hmac.Key
    };

    // Save newly created user
    _context.AppUsers.Add(user);
    await _context.SaveChangesAsync();

    return new UserDto
    {
      Username = user.UserName,
      Token = _tokenService.CreateToken(user),
    };
  }

  [HttpPost("login")]
  public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
  {
    var user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName.ToLower() == loginDto.UserName.ToLower());

    if (user == null) return Unauthorized("Invalid UserName");

    // Get the Hash value to convert the password
    using var hmac = new HMACSHA512(user.PasswordSalt);

    // Compute the Hash value with the Password supplied
    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

    // Check if the passwords match
    for (int i = 0; i < computedHash.Length; i++)
    {
      if (computedHash[i] != user.PasswordHash[i])
        return Unauthorized("Invalid Password");
    }

    // If this point is reached then passwords match and user is authorized access
    return new UserDto
    {
      Username = user.UserName,
      Token = _tokenService.CreateToken(user),
    };
  }

  private async Task<bool> UserExists(string username)
  {
    return await _context.AppUsers.AnyAsync(x => x.UserName.ToLower() == username.ToLower()); // Bob != bob
  }
}
