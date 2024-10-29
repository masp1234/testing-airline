using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/mysql/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _userService.GetUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to get users." });

            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserCreationRequest userCreationRequest)
        {
            try
            {
                UserResponse? user = await _userService.GetUserByEmail(userCreationRequest.Email);
                if (user != null)
                {
                    return Conflict(new { message = "User already exists with this email." });
                }

                await _userService.CreateUser(userCreationRequest);
                return StatusCode(StatusCodes.Status201Created, new { message = "User created successfully!" });
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while trying to create a new user." });

            }
        }
    }
}

