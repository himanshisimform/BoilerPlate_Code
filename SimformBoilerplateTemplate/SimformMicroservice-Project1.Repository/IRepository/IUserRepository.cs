using SimformMicroservice_Project1.Database.Tables;
using SimformMicroservice_Project1.DTO;

namespace SimformMicroservice_Project1.Repository.IRepository;

public interface IUserRepository
{
    Task<ApplicationUser?> GetUserByIdAsync(Guid userId);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
    Task<ApplicationUser?> GetUserWithProfileAsync(Guid userId);
    Task<bool> IsEmailTakenAsync(string email);
    Task<IEnumerable<ApplicationUser>> GetActiveUsersAsync();
    Task<PagedResult<ApplicationUser>> GetPagedUsersAsync(int pageNumber, int pageSize, string? searchTerm = null);
    Task<ApplicationUser> CreateUserAsync(ApplicationUser user);
    Task<ApplicationUser> UpdateUserAsync(ApplicationUser user);
    Task DeleteUserAsync(Guid userId);
}