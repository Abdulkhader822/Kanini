// Controllers/TokenController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProSportsStore.DTOs;
using ProSportsStore.IAuthentication;
using ProSportsStore.Interface;

namespace ProSportsStore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IUser _users;
        private readonly ITokenGenerate _token;

        public TokenController(IUser users, ITokenGenerate token)
        {
            _users = users;
            _token = token;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            var user = await _users.Login(login);
            if (user == null) return Unauthorized("Invalid credentials");

           

            var jwt = _token.GenerateToken(user.Email, user.Role);
            return Ok(new { token = jwt });
        }

      


    }
}
