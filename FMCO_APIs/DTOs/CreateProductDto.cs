using System.ComponentModel.DataAnnotations;

namespace ProductManagment_APIs.DTOs
{
    public class CreateProductDto
    {
        [Required]
        public string ProductName { get; set; } = string.Empty;

        public int? QTY { get; set; }
    }
}
