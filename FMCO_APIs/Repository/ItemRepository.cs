using Microsoft.EntityFrameworkCore;
using ProductManagment_APIs.Data;
using ProductManagment_APIs.DTOs;
using ProductManagment_APIs.Interface;
using ProductManagment_APIs.Model;

namespace ProductManagment_APIs.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;

        public ItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<ItemDto>> GetAllAsync(PagedRequest request)
        {
            var items = await _context.Items
                .AsNoTracking()
                .Include(x => x.Product)
                .Select(x => new ItemDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.ProductName,
                    Quantity = x.Quantity
                })
                .ToListAsync();

            return new PagedResponse<ItemDto>
            {
                TotalRecords = items.Count,
                TotalPages = 1,
                PageNumber = 1,
                PageSize = items.Count,
                Data = items
            };
        }

        public async Task<ItemDto?> GetByIdAsync(int id)
        {
            return await _context.Items
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.Id == id)
                .Select(x => new ItemDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.ProductName,
                    Quantity = x.Quantity
                })
                .FirstOrDefaultAsync();
        }

        public async Task<ItemDto?> GetByProductIdAsync(int id)
        {
            return await _context.Items
                .AsNoTracking()
                .Include(x => x.Product)
                .Where(x => x.ProductId == id)
                .Select(x => new ItemDto
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.ProductName,
                    Quantity = x.Quantity
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateAsync(CreateItemDto dto)
        {
            var productExists = await _context.Products
                .AnyAsync(x => x.Id == dto.ProductId);

            if (!productExists)
                return false;

            var item = new Item
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            };

            _context.Items.Add(item);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(int id, UpdateItemDto dto)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
                return false;

            item.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
                return false;

            _context.Items.Remove(item);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}