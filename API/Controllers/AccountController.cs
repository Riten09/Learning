using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : BaseApiController
{

    private readonly DataContext _dataContext;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)  // injecting token service & DBContext 
    {
        _tokenService = tokenService;
        _mapper = mapper;
        _dataContext = context;
    }

    [HttpPost("register")]  // POST: api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto register)
    {
        if (await UserExists(register.UserName)) return BadRequest("User is already exists.");

    var user = _mapper.Map<AppUser>(register);

        using var hmac= new HMACSHA512();
        user.UserName = register.UserName.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
        user.PasswordSalt = hmac.Key;
        _dataContext.Add(user);
        await _dataContext.SaveChangesAsync();

        return new UserDto{
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };    

    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(loginDto login)
    {
        
        var user = await _dataContext.Users
        .Include(p=>p.Photos)
        .SingleOrDefaultAsync(x => x.UserName == login.username.ToLower());

        if (user == null) return Unauthorized("invalid user name.");

        using var hmac= new HMACSHA512(user.PasswordSalt);

        var computehash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

        for (int i=0; i < computehash.Length; i++)
        {
            if (computehash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password.");
        }

        return new UserDto{
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain)?.Url,
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _dataContext.Users.AnyAsync(x => x.UserName == username.ToLower());
    }
}
