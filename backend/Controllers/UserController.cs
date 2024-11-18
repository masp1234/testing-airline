using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace backend.Controllers
{
	[ApiController]
	[Route("/api/mysql/[controller]")]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _userService;
		public UsersController(IUserService userService)
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
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occured while trying to get users." });

			}
		}
		[HttpPost]
		public async Task<IActionResult> AddUser([FromBody] UserCreationRequest userCreationRequest)
		{
			if (string.IsNullOrEmpty(userCreationRequest.Email) || string.IsNullOrEmpty(userCreationRequest.Password))
			{
				return BadRequest(new { message = "Email or password missing." });
			}
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
				Console.WriteLine(ex);
				SentrySdk.CaptureException(ex);
				return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while trying to create a new user." });

			}
		}
		[HttpPost("login")]
		
		public async Task<IActionResult> Login([FromBody] JwtRequest request)
		{
			var user = await _userService.CheckUserByEmail(request.Email);

			if (user == null) return NotFound(new {message = "User not exist!"});

			if (user != null)
			{
				// Initialize PasswordHasher to verify password
				Boolean passwordValidation =_userService.CheckPasswordValidation(request.Password, user.Password, user);
				// Check if the password is correct
				if (passwordValidation)
				{
					// Generate JWT token after successful password verification
					var token = _userService.GenerateJwtToken(user);

					Response.Cookies.Append("AuthToken", token, new CookieOptions
					{
						HttpOnly = true,
						Secure = false,
						SameSite = SameSiteMode.Strict,
					});

					return Ok(new { message = "Login successful: " });
				}
			}

			return Unauthorized();
		}
		
		[Authorize]
		[HttpGet("currentRole")]
		public IActionResult GetCurrentUserRole()
		{
			var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
			Console.WriteLine(roleClaim);

			if (roleClaim == null)
			{
				return Unauthorized(new { message = "User role not found." });
			}

		return Ok(new { role = roleClaim });
}
	}
}

