using API.DTOs;
using API.Entities;
using API.Extension;
using AutoMapper;

namespace API.Helpers;

public class AutomapperProfiles : Profile
{
    public AutomapperProfiles()
    {
        CreateMap<AppUser, MemberDTO>()
        .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(scr => scr.Photos.FirstOrDefault(x=>x.IsMain).Url))
        .ForMember(dest => dest.Age, opt => opt.MapFrom(scr => scr.DateOfBirth.CalculateAge()));
        CreateMap<Photo,PhotoDto>();

        CreateMap<MemberUpdatesDto,AppUser>();
    }
}
