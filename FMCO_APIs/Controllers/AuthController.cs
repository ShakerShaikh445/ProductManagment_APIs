using ProductManagment_APIs.Data;
using ProductManagment_APIs.DTOs;
using ProductManagment_APIs.Interface;
using ProductManagment_APIs.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProductManagment_APIs.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;

        public AuthController(AppDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.MasterUsers
                .Where(x => x.UserName == dto.UserName && x.IsActive)
                .FirstOrDefaultAsync();

            if (user == null)
                return Unauthorized("Invalid username or password");

            if (!PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid username or password");

            var loginResponse = await _jwtService.GenerateToken(user);
            return Ok(loginResponse);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto dto)
        {
            var result = await _jwtService.RefreshTokenAsync(dto);

            if (result == null)
                return Unauthorized("Invalid or expired refresh token.");

            return Ok(result);
        }

    }
       

}
