using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimformMicroservice_Project1.Database.Tables;
using SimformMicroservice_Project1.DTO;
using SimformMicroservice_Project1.Repository.IRepository;
using SimformMicroservice_Project1.Service.IService;

namespace SimformMicroservice_Project1.Service.Service;

public class UserService(
    IUserRepository userRepository,
    UserManager<ApplicationUser> userManager,
    IMapper mapper) : IUserService
{
    public async Task<BaseResponse<UserDto>> GetUserByIdAsync(Guid userId)
    {
        try
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                return BaseResponse<UserDto>.Failure("User not found or inactive");
            }

            var userDto = mapper.Map<UserDto>(user);
            userDto.Roles = (await userManager.GetRolesAsync(user)).ToList();

            return BaseResponse<UserDto>.Success(userDto, "User retrieved successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<UserDto>.Failure($"Failed to retrieve user: {ex.Message}");
        }
    }

    public async Task<BaseResponse<UserDto>> GetUserByEmailAsync(string email)
    {
        try
        {
            var user = await userRepository.GetUserByEmailAsync(email);
            if (user == null || !user.IsActive)
            {
                return BaseResponse<UserDto>.Failure("User not found or inactive");
            }

            var userDto = mapper.Map<UserDto>(user);
            userDto.Roles = (await userManager.GetRolesAsync(user)).ToList();

            return BaseResponse<UserDto>.Success(userDto, "User retrieved successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<UserDto>.Failure($"Failed to retrieve user: {ex.Message}");
        }
    }

    public async Task<BaseResponse<PagedResult<UserDto>>> GetUsersAsync(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
    {
        try
        {
            var users = await userRepository.GetPagedUsersAsync(pageNumber, pageSize, searchTerm);
            var userDtos = new List<UserDto>();

            foreach (var user in users.Items)
            {
                var userDto = mapper.Map<UserDto>(user);
                userDto.Roles = (await userManager.GetRolesAsync(user)).ToList();
                userDtos.Add(userDto);
            }

            var pagedResult = new PagedResult<UserDto>
            {
                Items = userDtos,
                TotalCount = users.TotalCount,
                PageNumber = users.PageNumber,
                PageSize = users.PageSize
            };

            return BaseResponse<PagedResult<UserDto>>.Success(pagedResult, "Users retrieved successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<PagedResult<UserDto>>.Failure($"Failed to retrieve users: {ex.Message}");
        }
    }

    public async Task<BaseResponse<UserDto>> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        try
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return BaseResponse<UserDto>.Failure("User not found");
            }

            // Update user properties
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            
            if (request.IsActive.HasValue)
            {
                user.IsActive = request.IsActive.Value;
            }

            var updatedUser = await userRepository.UpdateUserAsync(user);
            var userDto = mapper.Map<UserDto>(updatedUser);
            userDto.Roles = (await userManager.GetRolesAsync(updatedUser)).ToList();

            return BaseResponse<UserDto>.Success(userDto, "User updated successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<UserDto>.Failure($"Failed to update user: {ex.Message}");
        }
    }

    public async Task<BaseResponse<string>> DeleteUserAsync(Guid userId)
    {
        try
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return BaseResponse<string>.Failure("User not found");
            }

            await userRepository.DeleteUserAsync(userId);
            return BaseResponse<string>.Success("User deleted successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<string>.Failure($"Failed to delete user: {ex.Message}");
        }
    }

    public async Task<BaseResponse<string>> DeactivateUserAsync(Guid userId)
    {
        try
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return BaseResponse<string>.Failure("User not found");
            }

            user.IsActive = false;
            await userRepository.UpdateUserAsync(user);

            return BaseResponse<string>.Success("User deactivated successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<string>.Failure($"Failed to deactivate user: {ex.Message}");
        }
    }

    public async Task<BaseResponse<string>> ActivateUserAsync(Guid userId)
    {
        try
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return BaseResponse<string>.Failure("User not found");
            }

            user.IsActive = true;
            await userRepository.UpdateUserAsync(user);

            return BaseResponse<string>.Success("User activated successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<string>.Failure($"Failed to activate user: {ex.Message}");
        }
    }

    public async Task<BaseResponse<List<UserDto>>> GetActiveUsersAsync()
    {
        try
        {
            var users = await userRepository.GetActiveUsersAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var userDto = mapper.Map<UserDto>(user);
                userDto.Roles = (await userManager.GetRolesAsync(user)).ToList();
                userDtos.Add(userDto);
            }

            return BaseResponse<List<UserDto>>.Success(userDtos, "Active users retrieved successfully");
        }
        catch (Exception ex)
        {
            return BaseResponse<List<UserDto>>.Failure($"Failed to retrieve active users: {ex.Message}");
        }
    }
}