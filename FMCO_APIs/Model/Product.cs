using System.ComponentModel.DataAnnotations;

namespace ProductManagment_APIs.Model
{
    public class Product : Audit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
