using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class UserRepository : IUserRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UserRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
    {
        return await _context.Users
        .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
        .ToListAsync();
    }

    public async Task<MemberDTO> GetMembersByUserNameAsync(string username)
    {
        return await _context.Users
        .Where(x=>x.UserName == username.ToLower())
        .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _context.Users
        .Include(p => p.Photos)
        .FirstOrDefaultAsync(x=>x.Id == id); 
    }

    public async Task<AppUser> GetUserByUserNameAsync(string username)
    {
        return await _context.Users
        .Include(p => p.Photos)
        .SingleOrDefaultAsync(x => x.UserName == username.ToLower());
    }

    public async Task<IEnumerable<AppUser>> GetUsersAsync()
    {
        return await _context.Users
        .Include(p => p.Photos)
        .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0; // return 0 means false & greater than 0 true
    }

    public void update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

}
