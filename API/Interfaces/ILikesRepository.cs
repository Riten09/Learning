using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API;

public interface ILikesRepository
{
    Task<UserLike> GetUserLike(int sourceUserID, int targetUserID);
    Task<AppUser> GetUserWithLikes(int userId);
    Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    Task<bool> SaveAllAsync();

}
