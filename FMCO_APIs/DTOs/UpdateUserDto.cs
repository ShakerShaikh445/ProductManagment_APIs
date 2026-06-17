using System.ComponentModel.DataAnnotations;

namespace ProductManagment_APIs.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; }

        [Required]
        public List<int> RoleIds { get; set; } = new();
    }
}