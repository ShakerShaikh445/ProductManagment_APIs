using ProductManagment_APIs.Model;
using System.ComponentModel.DataAnnotations;

public class Role : Audit
{
    [Key]
    [Required, MaxLength(50)]
    public int RoleId { get; set; }

    [Required, MaxLength(50)]
    public string RoleName { get; set; } = string.Empty;

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
