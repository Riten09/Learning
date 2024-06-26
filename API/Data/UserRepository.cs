﻿using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
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

    public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
    {
        var query = _context.Users.AsQueryable();
        query = query.Where(u=> u.UserName != userParams.CurrentUsername);
        query = query.Where(g=> g.Gender == userParams.Gender);

        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(d => d.DateOfBirth >= minDob && d.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch{
            "created" => query.OrderByDescending (u=>u.CreatedOn),
            _=> query.OrderByDescending(u=>u.LastActive) //_ is repesent default
        };
        return await PagedList<MemberDTO>.CreateAsync(query.AsNoTracking().ProjectTo<MemberDTO>(_mapper.ConfigurationProvider)
        , userParams.PageNumber,userParams.PageSize);
    }

    public async Task<MemberDTO> GetMembersByUserNameAsync(string username)
    {
        return await _context.Users
        .Where(x=>x.UserName == username.ToLower())
        .ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
    }

    public async Task<AppUser> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
        
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
