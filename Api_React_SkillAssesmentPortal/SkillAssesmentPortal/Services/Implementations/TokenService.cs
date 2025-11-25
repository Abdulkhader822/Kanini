using Microsoft.IdentityModel.Tokens;
using SkillAssessmentPortal.Models;
using SkillAssessmentPortal.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkillAssessmentPortal.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration config, ILogger<TokenService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public string GenerateToken(User user)
        {
            _logger.LogInformation("Generating JWT token for UserId: {UserId}, Role: {Role}", user.UserId, user.Role);
            
            try
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var expiry = DateTime.Now.AddDays(2);
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: expiry,
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
                _logger.LogInformation("JWT token generated successfully for UserId: {UserId}, expires: {Expiry}", user.UserId, expiry);
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token for UserId: {UserId}", user.UserId);
                throw;
            }
        }
    }
}