using API.Data;
using API.DTOs;
using API.Entities;
using API.Extension;
using API.Helpers;
using Microsoft.EntityFrameworkCore;

namespace API;

public class LikesRepository : ILikesRepository
{
    private readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }
    public async Task<UserLike> GetUserLike(int sourceUserID, int targetUserID)
    {
        return await _context.Likes.FindAsync(sourceUserID, targetUserID);
    }

    public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
    {
        var users = _context.Users.OrderBy(u=>u.UserName).AsQueryable();
        var likes = _context.Likes.AsQueryable();

        if(likesParams.Predicate == "liked"){
            likes = likes.Where(likes => likes.SourceUserID == likesParams.UserId);
            users = likes.Select(like=> like.TargetUser);
        }
        if(likesParams.Predicate == "likedBy"){
            likes = likes.Where(likes => likes.TargetUserID == likesParams.UserId);
            users = likes.Select(like=> like.SourceUser);
        }

        var likedUsers =  users.Select(user => new LikeDto{  // doing maunal mapping of user & Likes DTO
            UserName = user.UserName,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(x=>x.IsMain).Url,
            City =user.City,
            Id = user.Id
        });
        return await PagedList<LikeDto>.CreateAsync(likedUsers,likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        return await _context.Users
        .Include(x=>x.LikedUsers)
        .FirstOrDefaultAsync(x=>x.Id == userId);
    }
      public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0; // return 0 means false & greater than 0 true
    }
}
