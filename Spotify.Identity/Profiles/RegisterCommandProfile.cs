using AutoMapper;
using Spotify.Identity.Entity;
using Spotify.Identity.Mediatr.Commands;

namespace Spotify.Identity.Profiles;

public class RegisterCommandProfile : Profile
{
    public RegisterCommandProfile()
    {
        CreateMap<RegisterCommand, UserEntity>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth));
    }
}