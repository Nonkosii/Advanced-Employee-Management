
using BaseLibrary.DTO;
using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Service.Interface;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUserAccount userAccount) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> CreateAsync(UserRegister user)
        {
            if (user == null) return BadRequest("Model is empty");
            var result = await userAccount.CreateAsync(user);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> SignInAsync(UserLogin user)
        {
            if (user == null) return BadRequest("Model is empty");
            var result = await userAccount.SignInAsync(user);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync(RefreshTokenDto token)
        {
            if (token == null) return BadRequest("Model is empty");
            var result = await userAccount.RefreshTokenAsync(token);
            return Ok(result);
        }
    }
}
