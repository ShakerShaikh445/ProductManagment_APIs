using ProductManagment_APIs.Data;
using ProductManagment_APIs.DTOs;
using ProductManagment_APIs.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProductManagment_APIs.Service
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;
        private readonly LogHelper _log;

        public JwtService(IConfiguration config, AppDbContext context, LogHelper log)
        {
            _config = config;
            _context = context;
            _log = log;
        }

        public async Task<LoginResponseDto> GenerateToken(AppUser user)
        {
            try
            {
                if (user == null)
                    throw new ArgumentNullException(nameof(user));

                var roles = await _context.MasterUsers
                    .AsNoTracking()
                    .Where(u => u.Id == user.Id)
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .SelectMany(u => u.UserRoles.Select(ur => ur.Role.RoleName))
                    .ToListAsync();

                var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));

                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

                var expires = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_config["Jwt:DurationInMinutes"]));

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    audience: _config["Jwt:Audience"],
                    claims: claims,
                    expires: expires,
                    signingCredentials: new SigningCredentials(
                        key, SecurityAlgorithms.HmacSha256)
                );

                var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                var refreshToken = GenerateRefreshToken();
                var refreshExpiry = DateTime.UtcNow.AddDays(7);

                var dbUser = await _context.MasterUsers.FirstOrDefaultAsync(x => x.Id == user.Id);
                if (dbUser != null)
                {
                    dbUser.RefreshToken = refreshToken;
                    dbUser.RefreshTokenExpiryTime = refreshExpiry;
                    _context.MasterUsers.Update(dbUser);
                    await _context.SaveChangesAsync();
                }

                _log.Log($"JWT generated for {user.UserName}");

                return new LoginResponseDto
                {
                    Token = accessToken,
                    Expiration = expires,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = refreshExpiry,
                    UserId = user.Id,
                    Name = user.UserName,
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                throw;
            }
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenDto dto)
        {
            try
            {
                var user = await _context.MasterUsers
                    .FirstOrDefaultAsync(x =>
                        x.Id == dto.UserId &&
                        x.RefreshToken == dto.RefreshToken);

                if (user == null)
                {
                    _log.Log("Invalid refresh token");
                    return null;
                }

                if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
                {
                    _log.Log("Refresh token expired");
                    return null;
                }

                _log.Log("Refresh token success");

                return await GenerateToken(user);
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                throw;
            }
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                + Guid.NewGuid().ToString("N");
        }
    }
}