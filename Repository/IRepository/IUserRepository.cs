using cs_apiEcommerce.Models;
using cs_apiEcommerce.Models.Dtos;

namespace cs_apiEcommerce.Repository.IRepository;

public interface IUserRepository
{
    ICollection<User> GetUsers();
    User? GetUser(int id);

    bool IsUniqueUser(string username);
    Task<UserLoginResponseDto> Login(UserLoginDto userloginDto);
    Task<User> Register(CreateUserDto createUserDto);
}
