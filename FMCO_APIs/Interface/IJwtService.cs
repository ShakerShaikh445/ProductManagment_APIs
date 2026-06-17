using ProductManagment_APIs.DTOs;

namespace ProductManagment_APIs.Interface
{
    public interface IJwtService
    {
        Task<LoginResponseDto> GenerateToken(AppUser user);
        Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenDto dto);
    }
}
