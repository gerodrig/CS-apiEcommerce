using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using cs_apiEcommerce.Models;
using cs_apiEcommerce.Models.Dtos;
using cs_apiEcommerce.Repository.IRepository;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace cs_apiEcommerce.Repository;

public class UserRepository(
    ApplicationDbContext db,
    IConfiguration configuration
) : IUserRepository
{

    public readonly ApplicationDbContext _db = db;
    private readonly string? secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
    public User? GetUser(int id)
    {
        return _db.Users.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<User> GetUsers()
    {
        return [.. _db.Users.OrderBy(u => u.Username)];
    }

    public bool IsUniqueUser(string username)
    {
        return !_db.Users.Any(u => u.Username.ToLower().Trim() == username.ToLower().Trim());
    }

    public async Task<UserLoginResponseDto> Login(UserLoginDto userloginDto)
    {
        if (string.IsNullOrEmpty(userloginDto.Username))
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Username is required"
            };
        }

        User? user = await _db.Users.FirstOrDefaultAsync<User>(u => u.Username.ToLower().Trim() == userloginDto.Username.ToLower().Trim());

        //? Verify user is not null and password is correct
        if (user == null || !BCrypt.Net.BCrypt.Verify(userloginDto.Password, user.Password))
        {
            return new UserLoginResponseDto()
            {
                Token = "",
                User = null,
                Message = "Username or password are not valid"
            };
        }

        //? JWT
        JwtSecurityTokenHandler handlerToken = new();
        if (string.IsNullOrWhiteSpace(secretKey))
        {
            throw new InvalidOperationException("SecretKey is not configured");
        }
        byte[] key = Encoding.UTF8.GetBytes(secretKey);
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity([
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
            ]),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        SecurityToken token = handlerToken.CreateToken(tokenDescriptor);

        return new UserLoginResponseDto()
        {
            Token = handlerToken.WriteToken(token),
            User = new UserRegisterDto()
            {
                Username = user.Username,
                Name = user.Name,
                Role = user.Role,
                Password = user.Password ?? ""
            },
            Message = "User logged successfully!."
        };

    }

    public async Task<User> Register(CreateUserDto createUserDto)
    {
        string encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        User user = new()
        {
            Username = createUserDto.Username ?? "No Username",
            Name = createUserDto.Name,
            Role = createUserDto.Role,
            Password = encryptedPassword

        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }
}
