using System.ComponentModel.DataAnnotations;

namespace ProductManagment_APIs.DTOs
{
    public class UpdateProductDto
    {
        [Required]
        public string ProductName { get; set; } = string.Empty;

        public UpdateItemDto? item { get; set; }
    }
}
