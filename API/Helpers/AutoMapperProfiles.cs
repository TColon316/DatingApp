using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
  public AutoMapperProfiles()
  {
    CreateMap<AppUser, MemberDto>()
      .ForMember(dest => dest.Age, options => options.MapFrom(s => s.DateOfBirth.CalculateAge()))
      .ForMember(dest => dest.PhotoUrl, options => options.MapFrom(source => source.Photos.FirstOrDefault(x => x.IsMain)!.URL));
    CreateMap<MemberDto, AppUser>();
    CreateMap<MemberUpdateDto, AppUser>().ReverseMap();
    CreateMap<Photo, PhotoDto>().ReverseMap();
  }
}
