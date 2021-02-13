using System.Threading.Tasks;
using LoginApp.Data;
using LoginApp.Dtos;
using LoginApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace LoginApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            registerDto.Name = registerDto.Name.ToLower();
            var role = "user";

            if (registerDto.AdminCode != null) {
                if (registerDto.AdminCode == _config.GetSection("AppSettings:AdminCode").Value)
                    role = "admin";
                else 
                    return Unauthorized("Admin confirmation unsuccessful");
            }

            if (await _repo.UserExists(registerDto.Name))
                return BadRequest("Username already exists");

            var userToRegister = new User()
            {
                Name = registerDto.Name,
                Role = role
            };

            var createdUser = await _repo.Register(userToRegister, registerDto.Password);

            var createdUserToReturn = new RegisteredUserDto() {
                Name = createdUser.Name,
                Role = createdUser.Role
            };

            return Ok(createdUserToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _repo.Login(loginDto.Name.ToLower(), loginDto.Password);
            
            if (user == null)
                return Unauthorized("user authentication failed");

            var userToReturn = new RegisteredUserDto() {
                Name = user.Name,
                Role = user.Role
            };

            return Ok(userToReturn);
        }
    }
}