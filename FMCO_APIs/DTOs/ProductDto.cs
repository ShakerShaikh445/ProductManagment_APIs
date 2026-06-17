namespace ProductManagment_APIs.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int TotalItems { get; set; }
        public bool? IsActive { get; set; } 
    }
}
