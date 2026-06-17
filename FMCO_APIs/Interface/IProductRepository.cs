using ProductManagment_APIs.DTOs;

namespace ProductManagment_APIs.Interface
{
    public interface IProductRepository
    {
        Task<PagedResponse<ProductDto>> GetAllAsync(PagedRequest request);

        Task<ProductDetailsDto?> GetByIdAsync(int id);

        Task<bool> CreateAsync(CreateProductDto dto);

        Task<bool> UpdateAsync(int id, UpdateProductDto dto);

        Task<bool> DeleteAsync(int id);

        Task<bool> ChangeStatusAsync(int id);
    }
}