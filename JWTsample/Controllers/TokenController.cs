using JWTsample.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTsample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly SymmetricSecurityKey _key;

        private readonly AddDbContext _con;
        
        public TokenController(AddDbContext con, IConfiguration configuration)
        {
            _key = new
            SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]!));
            _con = con;
        }
        [NonAction]
        public string GenerateToken(User user)
        {
            string token = string.Empty;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId,user.UserName!),
                new Claim(ClaimTypes.Role,user.Role),
            };
            var cred = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(2),
                SigningCredentials = cred
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var myToken = tokenHandler.CreateToken(tokenDescription);
            token = tokenHandler.WriteToken(myToken);
            return token;
        }

        [HttpPost]
        public async Task<IActionResult> Post(User userData)
        {
            if (userData != null && !string.IsNullOrEmpty(userData.Email) &&
           !string.IsNullOrEmpty(userData.Password) && !string.IsNullOrEmpty(userData.Role))
            {
                var user = await GetUser(userData.Email, userData.Password,userData.Role);
                if (user != null)
                {
                    var token = GenerateToken(user);
                    return Ok(new { token });
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest("Invalid request data");
            }

        }
        private async Task<User> GetUser(string email, string password,String Role)
        {
            return await _con.Users.FirstOrDefaultAsync(u => u.Email == email &&
           u.Password == password&& u.Role==u.Role) ?? new Models.User();
        }
    }
}



