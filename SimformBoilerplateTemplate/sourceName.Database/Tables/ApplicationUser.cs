using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace sourceName.Database.Tables;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual UserProfile? Profile { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    public string FullName => $"{FirstName} {LastName}".Trim();
}
