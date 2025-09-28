//using ArtAuction.Dto;
//using ArtAuction.Interface;
//using ArtAuction.Models;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace ArtAuction.Service
//{
//    public class AuthService : IAuthService
//    {
//        private readonly ArtAuctionDbContext _context;
//        private readonly IConfiguration _config;

//        public AuthService(ArtAuctionDbContext context, IConfiguration config)
//        {
//            _context = context;
//            _config = config;
//        }

//        public async Task<string?> AuthenticateAsync(UserLoginDto loginDto)
//        {
//            var user = await _context.Users
//                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

//            if (user == null) return null;

//            // ✅ Replace with BCrypt.Verify if passwords are hashed
//            if (user.PasswordHash != loginDto.Password)
//                return null;

//            // Create claims
//            var claims = new List<Claim>
//            {
//                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
//                new Claim(ClaimTypes.Name, user.FullName),
//                new Claim(ClaimTypes.Email, user.Email),
//                new Claim(ClaimTypes.Role, user.Role) // "Admin", "Artist", "Buyer"
//            };

//            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
//            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//            var token = new JwtSecurityToken(
//                issuer: _config["Jwt:Issuer"],
//                audience: _config["Jwt:Audience"],
//                claims: claims,
//                expires: DateTime.UtcNow.AddHours(2),
//                signingCredentials: creds
//            );

//            return new JwtSecurityTokenHandler().WriteToken(token);
//        }
//    }
//}
