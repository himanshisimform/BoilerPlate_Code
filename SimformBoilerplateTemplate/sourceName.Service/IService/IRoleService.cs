using sourceName.DTO;

namespace sourceName.Service.IService;

public interface IRoleService
{
    Task<BaseResponse<List<RoleDto>>> GetAllRolesAsync();
    Task<BaseResponse<RoleDto>> GetRoleByIdAsync(Guid roleId);
    Task<BaseResponse<RoleDto>> CreateRoleAsync(CreateRoleRequest request);
    Task<BaseResponse<RoleDto>> UpdateRoleAsync(Guid roleId, UpdateRoleRequest request);
    Task<BaseResponse<string>> DeleteRoleAsync(Guid roleId);
    Task<BaseResponse<List<UserDto>>> GetUsersInRoleAsync(Guid roleId);
    Task<BaseResponse<string>> AddUserToRoleAsync(Guid userId, Guid roleId);
    Task<BaseResponse<string>> RemoveUserFromRoleAsync(Guid userId, Guid roleId);
}
