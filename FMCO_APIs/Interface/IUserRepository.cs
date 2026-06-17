using ProductManagment_APIs.DTOs;

namespace ProductManagment_APIs.Interface
{
    public interface IUserRepository
    {
        Task<PagedResponse<UserDto>> GetAllAsync(PagedRequest request);
        Task<UserDto?> GetByIdAsync(int id);
        Task<bool> CreateAsync(CreateUserDto dto);
        Task<bool> UpdateAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangeStatusAsync(int id);
    }
}
