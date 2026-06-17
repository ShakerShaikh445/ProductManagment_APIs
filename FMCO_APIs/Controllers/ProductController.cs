using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManagment_APIs.DTOs;
using ProductManagment_APIs.Interface;

namespace ProductManagment_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        [HttpPost("list")]
        public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
        {
            var result = await _productRepository.GetAllAsync(request);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productRepository.GetByIdAsync(id);

            if (result == null)
                return NotFound("Product not found.");

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var result = await _productRepository.CreateAsync(dto);

            if (!result)
                return BadRequest("Product already exists.");

            return Ok(new
            {
                Success = true,
                Message = "Product created successfully."
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductDto dto)
        {
            var result = await _productRepository.UpdateAsync(id, dto);

            if (!result)
                return NotFound("Product not found.");

            return Ok(new
            {
                Success = true,
                Message = "Product updated successfully."
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productRepository.DeleteAsync(id);

            if (!result)
                return NotFound("Product not found.");

            return Ok(new
            {
                Success = true,
                Message = "Product deleted successfully."
            });
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id)
        {
            var result = await _productRepository.ChangeStatusAsync(id);

            if (!result)
                return NotFound("Product not found.");

            return Ok(new
            {
                Success = true,
                Message = "Product status updated successfully."
            });
        }
    }
}