using API.DTOs;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;
public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        //AppUser
        CreateMap<AppUser, MemberDto>()
            .ForMember(dest => dest.Age, orig => orig.MapFrom(source => source.DateOfBirth.CalculateAge()))
            .ForMember(dest => dest.PhotoUrl, orig => orig.MapFrom(source => source.Photos // Map the Photo to the owner
            .FirstOrDefault(x => x.IsMain)!.Url)).ReverseMap();

        //Member
        CreateMap<MemberUpdateDto, AppUser>().ReverseMap();

        //Photos
        CreateMap<Photo, PhotoDto>().ReverseMap();
    }
}