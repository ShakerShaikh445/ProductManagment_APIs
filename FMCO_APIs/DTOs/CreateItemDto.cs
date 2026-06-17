using System.ComponentModel.DataAnnotations;

namespace ProductManagment_APIs.DTOs
{
    public class CreateItemDto
    {
        
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}
