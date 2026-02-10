using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimformMicroservice_Project1.DTO;
using SimformMicroservice_Project1.Service.IService;
using System.Security.Claims;

namespace SimformMicroservice_Project1.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
[Produces("application/json")]
[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status403Forbidden)]
public class UsersController(IUserService userService) : ControllerBase
{
    /// <summary>
    /// Get current user details
    /// </summary>
    /// <returns>Current user information</returns>
    [HttpGet("me")]
    [ProducesResponseType(typeof(BaseResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse<UserDto>>> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return BadRequest(BaseResponse<UserDto>.Failure("Invalid user token"));
        }

        var result = await userService.GetUserByIdAsync(userId);
        
        if (result.IsSuccess)
            return Ok(result);
            
        return NotFound(result);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>User details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponse<UserDto>>> GetUser(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(BaseResponse<UserDto>.Failure("Invalid user ID"));
        }

        var result = await userService.GetUserByIdAsync(id);
        
        if (result.IsSuccess)
            return Ok(result);
            
        return NotFound(result);
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    /// <param name="email">User email</param>
    /// <returns>User details</returns>
    [HttpGet("by-email/{email}")]
    [ProducesResponseType(typeof(BaseResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponse<UserDto>>> GetUserByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(BaseResponse<UserDto>.Failure("Email is required"));
        }

        var result = await userService.GetUserByEmailAsync(email);
        
        if (result.IsSuccess)
            return Ok(result);
            
        return NotFound(result);
    }

    /// <summary>
    /// Get paginated list of users
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10, max: 100)</param>
    /// <param name="searchTerm">Search term for filtering users</param>
    /// <returns>Paginated list of users</returns>
    [HttpGet]
    [ProducesResponseType(typeof(BaseResponse<PagedResult<UserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BaseResponse<PagedResult<UserDto>>>> GetUsers(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        if (pageNumber < 1)
            pageNumber = 1;

        if (pageSize < 1)
            pageSize = 10;
        else if (pageSize > 100)
            pageSize = 100;

        var result = await userService.GetUsersAsync(pageNumber, pageSize, searchTerm);
        return Ok(result);
    }

    /// <summary>
    /// Get all active users
    /// </summary>
    /// <returns>List of active users</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(BaseResponse<List<UserDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<BaseResponse<List<UserDto>>>> GetActiveUsers()
    {
        var result = await userService.GetActiveUsersAsync();
        return Ok(result);
    }

    /// <summary>
    /// Update current user profile
    /// </summary>
    /// <param name="request">Update user request</param>
    /// <returns>Updated user details</returns>
    [HttpPut("me")]
    [ProducesResponseType(typeof(BaseResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse<UserDto>>> UpdateCurrentUser([FromBody] UpdateUserRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return BadRequest(BaseResponse<UserDto>.Failure("Invalid user token"));
        }

        var result = await userService.UpdateUserAsync(userId, request);
        
        if (result.IsSuccess)
            return Ok(result);
            
        return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Update user by ID (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="request">Update user request</param>
    /// <returns>Updated user details</returns>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BaseResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse<UserDto>>> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(BaseResponse<UserDto>.Failure("Invalid user ID"));
        }

        var result = await userService.UpdateUserAsync(id, request);
        
        if (result.IsSuccess)
            return Ok(result);
            
        return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Deactivate user (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{id:guid}/deactivate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse<string>>> DeactivateUser(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(BaseResponse<string>.Failure("Invalid user ID"));
        }

        var result = await userService.DeactivateUserAsync(id);
        
        if (result.IsSuccess)
            return Ok(result);
            
        return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Activate user (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success message</returns>
    [HttpPost("{id:guid}/activate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse<string>>> ActivateUser(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(BaseResponse<string>.Failure("Invalid user ID"));
        }

        var result = await userService.ActivateUserAsync(id);
        
        if (result.IsSuccess)
            return Ok(result);
            
        return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }

    /// <summary>
    /// Delete user permanently (Admin only)
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Success message</returns>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BaseResponse<string>>> DeleteUser(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest(BaseResponse<string>.Failure("Invalid user ID"));
        }

        var result = await userService.DeleteUserAsync(id);
        
        if (result.IsSuccess)
            return Ok(result);
            
        return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
    }
}