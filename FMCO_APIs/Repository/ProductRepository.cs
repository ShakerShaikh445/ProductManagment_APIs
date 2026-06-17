using Microsoft.EntityFrameworkCore;
using ProductManagment_APIs.Data;
using ProductManagment_APIs.DTOs;
using ProductManagment_APIs.Interface;
using ProductManagment_APIs.Model;
using ProductManagment_APIs.Service;

namespace ProductManagment_APIs.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        private readonly IItemRepository _itemRepository;
        private readonly LogHelper _log;

        public ProductRepository(AppDbContext context, IItemRepository itemRepository,LogHelper log)
        {
            _context = context;
            _itemRepository = itemRepository;
            _log = log;
        }

        public async Task<PagedResponse<ProductDto>> GetAllAsync(PagedRequest request)
        {
            try
            {
                var query = _context.Products
                                            .AsNoTracking()
                                            .Include(x => x.Items)
                                            .AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    var search = request.SearchValue.Trim().ToLower();

                    query = query.Where(x => x.ProductName.ToLower().Contains(search));
                }

                query = request.SortColumn?.ToLower() switch
                {
                    "productname" => request.SortDirection?.ToLower() == "desc"
                        ? query.OrderByDescending(x => x.ProductName)
                        : query.OrderBy(x => x.ProductName),

                    "createdat" => request.SortDirection?.ToLower() == "asc"
                        ? query.OrderBy(x => x.CreatedAT)
                        : query.OrderByDescending(x => x.CreatedAT),

                    _ => query.OrderByDescending(x => x.CreatedAT)
                };

                var totalRecords = await query.CountAsync();

                if (request.PageSize > 0)
                {
                    query = query.Skip((request.PageNumber - 1) * request.PageSize)
                                 .Take(request.PageSize);
                }

                var data = await query
                                    .Select(x => new ProductDto
                                    {
                                        Id = x.Id,
                                        ProductName = x.ProductName,
                                        IsActive = x.IsActive,
                                        TotalItems = x.Items.Sum(i => i.Quantity)
                                    })
                                    .ToListAsync();

                _log.Log("Fetched Product List.");

                return new PagedResponse<ProductDto>
                {
                    TotalRecords = totalRecords,
                    TotalPages = request.PageSize == 0 ? 1 : (int)Math.Ceiling((double)totalRecords / request.PageSize),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    Data = data
                };
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                throw;
            }
        }
       
        
        public async Task<ProductDetailsDto?> GetByIdAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .AsNoTracking()
                    .Include(x => x.Items)
                    .Where(x => x.Id == id)
                    .Select(x => new ProductDetailsDto
                    {
                        Id = x.Id,
                        ProductName = x.ProductName,
                        IsActive = x.IsActive,
                        Items = x.Items.Select(i => new ItemDto
                        {
                            Id = i.Id,
                            ProductId = i.ProductId,
                            ProductName = x.ProductName,
                            Quantity = i.Quantity
                        }).ToList()
                    })
                    .FirstOrDefaultAsync();

                _log.Log($"Fetched Product Id : {id}");

                return product;
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> CreateAsync(CreateProductDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (await _context.Products.AnyAsync(x => x.ProductName == dto.ProductName))
                    return false;

                var product = new Product
                {
                    ProductName = dto.ProductName
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                if (dto.QTY != null)
                {
                    CreateItemDto dtItem = new CreateItemDto();
                    dtItem.ProductId = product.Id;
                    dtItem.Quantity = dto.QTY.Value;
                    await _itemRepository.CreateAsync(dtItem);
                }

                await transaction.CommitAsync();

                _log.Log($"Product Created : {product.ProductName}");

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateProductDto dto)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                    return false;

                product.ProductName = dto.ProductName;

                await _context.SaveChangesAsync();

                _log.Log($"Product Updated : {id}");

                return true;
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var product = await _context.Products
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (product == null)
                    return false;

                _context.Items.RemoveRange(product.Items);
                _context.Products.Remove(product);

                await _context.SaveChangesAsync();

                _log.Log($"Product Deleted : {id}");

                return true;
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                throw;
            }
        }

        public async Task<bool> ChangeStatusAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                    return false;

                product.IsActive = !product.IsActive;

                await _context.SaveChangesAsync();

                _log.Log($"Product Status Changed : {id}");

                return true;
            }
            catch (Exception ex)
            {
                _log.Exception(ex);
                throw;
            }
        }
    }
}