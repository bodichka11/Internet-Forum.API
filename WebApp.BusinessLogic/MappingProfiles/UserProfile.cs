using AutoMapper;
using WebApp.BusinessLogic.DTOs.User;
using WebApp.DataAccess.Entities;

namespace WebApp.BusinessLogic.MappingProfiles;
public class UserProfile : Profile
{
    public UserProfile()
    {
        _ = this.CreateMap<User, UserDto>();
        _ = this.CreateMap<UserDto, User>();
        _ = this.CreateMap<UserUpdateDto, User>();
        _ = this.CreateMap<UserUpdateDto, UserDto>();
        _ = this.CreateMap<UserDto, UserUpdateDto>();
        _ = this.CreateMap<User, UserUpdateDto>();
    }
}
