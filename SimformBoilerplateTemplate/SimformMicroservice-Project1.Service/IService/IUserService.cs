using SimformMicroservice_Project1.DTO;

namespace SimformMicroservice_Project1.Service.IService;

public interface IUserService
{
    Task<BaseResponse<UserDto>> GetUserByIdAsync(Guid userId);
    Task<BaseResponse<UserDto>> GetUserByEmailAsync(string email);
    Task<BaseResponse<PagedResult<UserDto>>> GetUsersAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null);
    Task<BaseResponse<UserDto>> UpdateUserAsync(Guid userId, UpdateUserRequest request);
    Task<BaseResponse<string>> DeleteUserAsync(Guid userId);
    Task<BaseResponse<string>> DeactivateUserAsync(Guid userId);
    Task<BaseResponse<string>> ActivateUserAsync(Guid userId);
    Task<BaseResponse<List<UserDto>>> GetActiveUsersAsync();
}