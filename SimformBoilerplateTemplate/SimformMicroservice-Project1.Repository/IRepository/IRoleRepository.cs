using SimformMicroservice_Project1.Database.Tables;

namespace SimformMicroservice_Project1.Repository.IRepository;

public interface IRoleRepository
{
    Task<ApplicationRole?> GetRoleByIdAsync(Guid roleId);
    Task<ApplicationRole?> GetRoleByNameAsync(string roleName);
    Task<IEnumerable<ApplicationRole>> GetAllRolesAsync();
    Task<IEnumerable<ApplicationRole>> GetActiveRolesAsync();
    Task<IEnumerable<ApplicationRole>> GetUserRolesAsync(Guid userId);
    Task<bool> RoleExistsAsync(string roleName);
    Task<ApplicationRole> CreateRoleAsync(ApplicationRole role);
    Task<ApplicationRole> UpdateRoleAsync(ApplicationRole role);
    Task DeleteRoleAsync(Guid roleId);
}