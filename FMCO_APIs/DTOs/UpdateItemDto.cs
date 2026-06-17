using System.ComponentModel.DataAnnotations;

namespace ProductManagment_APIs.DTOs
{
    public class UpdateItemDto
    {
        [Required]
        public int Quantity { get; set; }
    }
}
