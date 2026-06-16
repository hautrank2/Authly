using Authly.Models;
using Authly.Models.Dtos;
using Authly.Services.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Authly.Services
{
    public class AuthService(
        IOptions<AuthlyDatabaseSettings> dbSettings,
        IOptions<JwtSettings> jwtSettings) : IAuthService
    {
        private readonly IMongoCollection<User> _users = new MongoClient(dbSettings.Value.ConnectionString)
            .GetDatabase(dbSettings.Value.DatabaseName)
            .GetCollection<User>(dbSettings.Value.UsersCollectionName);

        private readonly JwtSettings _jwt = jwtSettings.Value;

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // Tìm user theo username
            var user = await _users
                .Find(u => u.Username == request.Username)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Username or password is incorrect");

            // Verify password bằng BCrypt
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
                throw new UnauthorizedAccessException("Username or password is incorrect");

            // Cập nhật LatestAccess
            var update = Builders<User>.Update.Set(u => u.LatestAccess, DateTime.UtcNow);
            await _users.UpdateOneAsync(u => u.Id == user.Id, update);

            // Tạo JWT token
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes);
            var token = GenerateToken(user, expiresAt);

            return new LoginResponseDto
            {
                AccessToken = token,
                ExpiresAt = expiresAt,
                User = UserDto.FromUser(user),
            };
        }

        private string GenerateToken(User user, DateTime expiresAt)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id!),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
