using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BCrypt.Net;
using cs_apiEcommerce.Models;
using cs_apiEcommerce.Models.Dtos;
using cs_apiEcommerce.Repository.IRepository;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace cs_apiEcommerce.Repository;

public class UserRepository(
    ApplicationDbContext db,
    IConfiguration configuration,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager,
    IMapper mapper
) : IUserRepository
{

    public readonly ApplicationDbContext _db = db;
    private readonly string? secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IMapper _mapper = mapper;
    public ApplicationUser? GetUser(string id)
    {
        return _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
    }

    public ICollection<ApplicationUser> GetUsers()
    {
        return [.. _db.ApplicationUsers.OrderBy(u => u.UserName)];
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

        ApplicationUser? user = await _db.ApplicationUsers.FirstOrDefaultAsync<ApplicationUser>(u => u.UserName != null && u.UserName.ToLower().Trim() == userloginDto.Username.ToLower().Trim());

        //? Verify user is not null and password is correct with Bcrypt
        // if (user == null || !BCrypt.Net.BCrypt.Verify(userloginDto.Password, user.Password))
        // {
        //     return new UserLoginResponseDto()
        //     {
        //         Token = "",
        //         User = null,
        //         Message = "Username or password are not valid"
        //     };
        // }

        //? Verify user not null using ASPCore Identity
        if (userloginDto.Password == null)
        {
            return new UserLoginResponseDto
            {
                Token = "",
                User = null,
                Message = "Username or password are not valid"
            };
        }

        if (user == null)
        {
            return new UserLoginResponseDto
            {
                Token = "",
                User = null,
                Message = "Username or password are not valid"
            };
        }

        bool isPasswordValid = await _userManager.CheckPasswordAsync(user, userloginDto.Password);

        if (!isPasswordValid)
        {
            return new UserLoginResponseDto
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
        //? Get the user roles
        IList<string>? roles = await _userManager.GetRolesAsync(user);
        byte[] key = Encoding.UTF8.GetBytes(secretKey);
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity([
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? string.Empty),
            ]),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        SecurityToken token = handlerToken.CreateToken(tokenDescriptor);

        return new UserLoginResponseDto()
        {
            Token = handlerToken.WriteToken(token),
            User = _mapper.Map<UserDataDto>(user),
            Message = "User logged successfully!."
        };

    }

    //? Before ASPNetCore Identity implementation
    // public async Task<User> Register(CreateUserDto createUserDto)
    // {
    //     string encryptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
    //     User user = new()
    //     {
    //         Username = createUserDto.Username ?? "No Username",
    //         Name = createUserDto.Name,
    //         Role = createUserDto.Role,
    //         Password = encryptedPassword

    //     };
    //     _db.Users.Add(user);
    //     await _db.SaveChangesAsync();
    //     return user;
    // }
    public async Task<UserDataDto> Register(CreateUserDto createUserDto)
    {
        if (string.IsNullOrEmpty(createUserDto.Username))
        {
            throw new ArgumentNullException("Username or password is required");
        }

        if (createUserDto.Password == null)
        {
            throw new ArgumentNullException("Username or password is required");
        }

        ApplicationUser user = new()
        {
            UserName = createUserDto.Username,
            Email = createUserDto.Username,
            NormalizedEmail = createUserDto.Username.ToUpper(),
            Name = createUserDto.Name
        };

        IdentityResult? result = await _userManager.CreateAsync(user, createUserDto.Password);
        if (result.Succeeded)
        {
            string? userRole = createUserDto.Role ?? "User";
            bool roleExists = await _roleManager.RoleExistsAsync(userRole);

            if (!roleExists)
            {
                IdentityRole identityRole = new(userRole);
                await _roleManager.CreateAsync(identityRole);
            }

            //? Assign the role to the user
            await _userManager.AddToRoleAsync(user, userRole);

            //? return the user that was created
            ApplicationUser? createdUser = _db.ApplicationUsers.FirstOrDefault(u => u.UserName == createUserDto.Username);
            return _mapper.Map<UserDataDto>(createdUser);
        }

        string? errors = string.Join(", ", result.Errors.Select(e => e.Description));
        throw new ApplicationException($"Registry could not be created: {errors}");
    }
}
