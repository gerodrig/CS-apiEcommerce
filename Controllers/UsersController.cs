using AutoMapper;
using cs_apiEcommerce.Models;
using cs_apiEcommerce.Models.Dtos;
using cs_apiEcommerce.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace cs_apiEcommerce.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(
        IUserRepository userRepository,
        IMapper mapper
    ) : ControllerBase
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsers()
        {
            ICollection<User> users = _userRepository.GetUsers();
            List<UserDto> usersDto = _mapper.Map<List<UserDto>>(users);

            return Ok(usersDto);
        }

        [HttpGet("{id:int}", Name = "GetUser")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUser(int id)
        {
            User? user = _userRepository.GetUser(id);

            if (user == null)
            {
                return NotFound($"The user with id {id} was not found.");
            }

            UserDto userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [AllowAnonymous]
        [HttpPost(Name = "RegisterUser")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDto createUserDto)
        {
            if (createUserDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(createUserDto.Username))
            {
                return BadRequest("Username is required.");
            }

            if (!_userRepository.IsUniqueUser(createUserDto.Username))
            {
                return BadRequest("The username is already taken, please select another username.");
            }
            User result = await _userRepository.Register(createUserDto);

            if (result == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "There was an error while trying to register, Please try again later.");
            }
            return CreatedAtRoute("GetUser", new { id = result.Id }, result);
        }

        [AllowAnonymous]
        [HttpPost("Login", Name = "LoginUser")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginUser([FromBody] UserLoginDto userLoginDto)
        {
            if (userLoginDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            UserLoginResponseDto user = await _userRepository.Login(userLoginDto);
            if (user == null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }
    }


}
