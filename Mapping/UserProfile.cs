using AutoMapper;
using cs_apiEcommerce.Models;
using cs_apiEcommerce.Models.Dtos;

namespace cs_apiEcommerce.Mapping;

public class UserProfile : Profile
{

    public UserProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, CreateUserDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserLoginDto>().ReverseMap();
        CreateMap<User, UserLoginResponseDto>().ReverseMap();
    }
}
