using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimformMicroservice_Project1.Database;
using SimformMicroservice_Project1.Database.Tables;
using SimformMicroservice_Project1.Repository.IRepository;

namespace SimformMicroservice_Project1.Repository;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleRepository(AppDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<ApplicationRole?> GetRoleByIdAsync(Guid roleId)
    {
        return await _context.Roles.FindAsync(roleId);
    }

    public async Task<ApplicationRole?> GetRoleByNameAsync(string roleName)
    {
        return await _roleManager.FindByNameAsync(roleName);
    }

    public async Task<IEnumerable<ApplicationRole>> GetAllRolesAsync()
    {
        return await _context.Roles.OrderBy(r => r.Name).ToListAsync();
    }

    public async Task<IEnumerable<ApplicationRole>> GetActiveRolesAsync()
    {
        return await _context.Roles
            .Where(r => r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<ApplicationRole>> GetUserRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return [];

        var roleNames = await _userManager.GetRolesAsync(user);
        var roles = new List<ApplicationRole>();

        foreach (var roleName in roleNames)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                roles.Add(role);
            }
        }

        return roles;
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await _roleManager.RoleExistsAsync(roleName);
    }

    public async Task<ApplicationRole> CreateRoleAsync(ApplicationRole role)
    {
        await _roleManager.CreateAsync(role);
        return role;
    }

    public async Task<ApplicationRole> UpdateRoleAsync(ApplicationRole role)
    {
        await _roleManager.UpdateAsync(role);
        return role;
    }

    public async Task DeleteRoleAsync(Guid roleId)
    {
        var role = await _context.Roles.FindAsync(roleId);
        if (role != null)
        {
            await _roleManager.DeleteAsync(role);
        }
    }
}