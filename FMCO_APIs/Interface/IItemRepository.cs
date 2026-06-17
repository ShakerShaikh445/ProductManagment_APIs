using ProductManagment_APIs.DTOs;

namespace ProductManagment_APIs.Interface
{
    public interface IItemRepository
    {
        Task<PagedResponse<ItemDto>> GetAllAsync(PagedRequest request);

        Task<ItemDto?> GetByIdAsync(int id);

        Task<bool> CreateAsync(CreateItemDto dto);

        Task<bool> UpdateAsync(int id, UpdateItemDto dto);

        Task<bool> DeleteAsync(int id);

        Task<ItemDto?> GetByProductIdAsync(int id);
    }
}
