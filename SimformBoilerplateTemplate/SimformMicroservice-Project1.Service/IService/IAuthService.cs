using SimformMicroservice_Project1.DTO;

namespace SimformMicroservice_Project1.Service.IService;

public interface IAuthService
{
    Task<BaseResponse<LoginResponse>> LoginAsync(LoginRequest request);
    Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request);
    Task<BaseResponse<string>> LogoutAsync(Guid userId);
    Task<BaseResponse<UserDto>> RegisterAsync(RegisterRequest request);
    Task<BaseResponse<string>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
}