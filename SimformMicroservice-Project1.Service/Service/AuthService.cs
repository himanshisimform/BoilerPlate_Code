using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using SimformMicroservice_Project1.Database.Tables;
using SimformMicroservice_Project1.Repository.IRepository;
using SimformMicroservice_Project1.Service.IService;
using SimformMicroservice_Project1.DTO;

namespace SimformMicroservice_Project1.Service.Service;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IConfiguration configuration,
    ITokenRepository tokenRepository,
    IUserRepository userRepository,
    IMapper mapper) : IAuthService
{
    public async Task<BaseResponse<LoginResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null || !user.IsActive)
            {
                return BaseResponse<LoginResponse>.Failure("Invalid email or password");
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                var message = result.IsLockedOut ? "Account is locked out" :
                             result.IsNotAllowed ? "Account is not activated" :
                             "Invalid email or password";

                return BaseResponse<LoginResponse>.Failure(message);
            }

            var tokenResponse = await GenerateTokenAsync(user);
            var userDto = mapper.Map<UserDto>(user);
            userDto.Roles = (await userManager.GetRolesAsync(user)).ToList();

            var loginResponse = new LoginResponse
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                ExpiresAt = tokenResponse.ExpiresAt,
                TokenType = tokenResponse.TokenType,
                User = userDto
            };

            return BaseResponse<LoginResponse>.Success(loginResponse, "Login successful");
        }
        catch (Exception ex)
        {
            return BaseResponse<LoginResponse>.Failure($"Login failed: {ex.Message}");
        }
    }

    public async Task<BaseResponse<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request)
    {
        try
        {
            var refreshToken = await tokenRepository.GetRefreshTokenAsync(request.RefreshToken);
            if (refreshToken == null || !refreshToken.IsActive)
            {
                return BaseResponse<TokenResponse>.Failure("Invalid refresh token");
            }

            var user = refreshToken.User;
            if (!user.IsActive)
            {
                return BaseResponse<TokenResponse>.Failure("User account is deactivated");
            }

            // Revoke the old refresh token
            await tokenRepository.RevokeRefreshTokenAsync(refreshToken.Token, "API", "Replaced by new token");

            var newTokenResponse = await GenerateTokenAsync(user);
            return BaseResponse<TokenResponse>.Success(newTokenResponse, "Token refreshed successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<TokenResponse>.Failure($"Token refresh failed: {ex.Message}");
        }
    }

    public async Task<BaseResponse<string>> LogoutAsync(Guid userId)
    {
        try
        {
            await tokenRepository.RevokeAllUserTokensAsync(userId, "API", "User logout");
            return BaseResponse<string>.Success("Logged out successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<string>.Failure($"Logout failed: {ex.Message}");
        }
    }

    public async Task<BaseResponse<UserDto>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            if (await userRepository.IsEmailTakenAsync(request.Email))
            {
                return BaseResponse<UserDto>.Failure("Email is already registered");
            }

            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BaseResponse<UserDto>.Failure("Registration failed", errors);
            }

            await userManager.AddToRoleAsync(user, "User");

            var userDto = mapper.Map<UserDto>(user);
            userDto.Roles = ["User"];

            return BaseResponse<UserDto>.Success(userDto, "Registration successful");
        }
        catch (Exception ex)
        {
            return BaseResponse<UserDto>.Failure($"Registration failed: {ex.Message}");
        }
    }

    public async Task<BaseResponse<string>> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return BaseResponse<string>.Failure("User not found");
            }

            var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BaseResponse<string>.Failure("Password change failed", errors);
            }

            await tokenRepository.RevokeAllUserTokensAsync(userId, "API", "Password changed");
            return BaseResponse<string>.Success("Password changed successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<string>.Failure($"Password change failed: {ex.Message}");
        }
    }

    private async Task<TokenResponse> GenerateTokenAsync(ApplicationUser user)
    {
        var claims = await GetClaimsAsync(user);
        var accessToken = GenerateAccessToken(claims);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedByIp = "API"
        };

        await tokenRepository.SaveRefreshTokenAsync(refreshTokenEntity);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            TokenType = "Bearer",
            ExpiresIn = 3600
        };
    }

    private string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    private async Task<List<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new("jti", Guid.NewGuid().ToString())
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }
}