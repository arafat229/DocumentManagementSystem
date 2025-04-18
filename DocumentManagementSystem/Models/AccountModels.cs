using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagementSystem.Model;


[Index(nameof(Email), new[] { nameof(UserName), nameof(PhoneNumber) }, IsUnique = true)]
public class User : IdentityUser<long>
{
    public string FullName { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; }
    [NotMapped]
    public IList<string> Roles { get; set; } = [];

    public virtual ICollection<UserRole> UserRoles { get; set; } = [];

}
[Index(nameof(Name), IsUnique = true)]
public class Role : IdentityRole<long>
{
    public DateTime createdAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdateAt { get; set; } = DateTime.UtcNow;
    public virtual ICollection<UserRole> UserRoles { get; set; } = [];


}

public class RoleClaim : IdentityRoleClaim<long>
{
    public virtual Role Role { get; set; } = null!;
}


public class UserRole : IdentityUserRole<long>
{
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;

}
