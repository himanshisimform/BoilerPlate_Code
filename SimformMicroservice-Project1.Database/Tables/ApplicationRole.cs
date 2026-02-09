using Microsoft.AspNetCore.Identity;

namespace SimformMicroservice_Project1.Database.Tables;

public class ApplicationRole : IdentityRole<Guid>
{
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public ApplicationRole() : base()
    {
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
    }

    public ApplicationRole(string roleName, string description) : this(roleName)
    {
        Description = description;
    }
}