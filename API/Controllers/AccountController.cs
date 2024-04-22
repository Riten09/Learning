using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{

    private readonly DataContext _dataContext;
    private readonly ITokenService _tokenService;
    public AccountController(DataContext context, ITokenService tokenService)  // injecting token service & DBContext 
    {
        _tokenService = tokenService;
        _dataContext = context;
    }

    [HttpPost("register")]  // POST: api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto register)
    {
        if (await UserExists(register.UserName)) return BadRequest("User is already exists.");
        using var hmac= new HMACSHA512();
        AppUser user = new AppUser(); 
        user.UserName = register.UserName.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
        user.PasswordSalt = hmac.Key;
        _dataContext.Add(user);
        await _dataContext.SaveChangesAsync();

        return new UserDto{
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };    

    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(loginDto login)
    {
        
        var user = await _dataContext.Users.SingleOrDefaultAsync(x => x.UserName == login.username.ToLower());

        if (user == null) return Unauthorized("invalid user name.");

        using var hmac= new HMACSHA512(user.PasswordSalt);

        var computehash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

        for (int i=0; i < computehash.Length; i++)
        {
            if (computehash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password.");
        }

        return new UserDto{
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _dataContext.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}
