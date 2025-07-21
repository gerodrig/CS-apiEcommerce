using cs_apiEcommerce.Models;
using cs_apiEcommerce.Models.Dtos;

namespace cs_apiEcommerce.Repository.IRepository;

public interface IUserRepository
{
    ICollection<ApplicationUser> GetUsers();
    ApplicationUser? GetUser(string id);

    bool IsUniqueUser(string username);
    Task<UserLoginResponseDto> Login(UserLoginDto userloginDto);
    Task<UserDataDto> Register(CreateUserDto createUserDto);
}
