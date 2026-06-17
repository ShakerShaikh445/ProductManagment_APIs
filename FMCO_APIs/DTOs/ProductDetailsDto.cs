namespace ProductManagment_APIs.DTOs
{
    public class ProductDetailsDto
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = string.Empty;
        public bool? IsActive { get; set; }

        public List<ItemDto> Items { get; set; } = new();
    }
}
