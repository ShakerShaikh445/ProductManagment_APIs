using ProductManagment_APIs.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

public class UserRole : Audit
{
    [Required]
    public int Id { get; set; }
    public virtual AppUser User { get; set; } = null!;

    [Required]
    public int RoleId { get; set; }
    public virtual Role Role { get; set; } = null!;
}
