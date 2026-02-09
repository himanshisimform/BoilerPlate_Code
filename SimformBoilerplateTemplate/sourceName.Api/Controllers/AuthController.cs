using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sourceName.DTO;
using sourceName.Service.IService;
using System.Security.Claims;

namespace sourceName.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// User login
    /// </summary>
    /// <param name="request">Login credentials</param>
    /// <returns>JWT tokens and user information</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(BaseResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        
        if (result.IsSuccess)
            return Ok(result);

        return result.Message.Contains("Invalid") || result.Message.Contains("locked") 
            ? Unauthorized(result) 
            : BadRequest(result);
    }

    /// <summary>
    /// User registration
    /// </summary>
    /// <param name="request">Registration details</param>
    /// <returns>Created user information</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(BaseResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BaseResponse<UserDto>>> Register([FromBody] RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request);
        
        if (result.IsSuccess)
            return CreatedAtAction(nameof(Register), result);

        return result.Message.Contains("already registered") 
            ? Conflict(result) 
            : BadRequest(result);
    }

    /// <summary>
    /// Refresh access token
    /// </summary>
    /// <param name="request">Refresh token</param>
    /// <returns>New JWT tokens</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(BaseResponse<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<TokenResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await authService.RefreshTokenAsync(request);
        
        if (result.IsSuccess)
            return Ok(result);

        return result.Message.Contains("Invalid") 
            ? Unauthorized(result) 
            : BadRequest(result);
    }

    /// <summary>
    /// User logout - revokes all refresh tokens
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<string>>> Logout()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return BadRequest(BaseResponse<string>.Failure("Invalid user token"));
        }

        var result = await authService.LogoutAsync(userId);
        
        if (result.IsSuccess)
            return Ok(result);
            
        return BadRequest(result);
    }

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    /// <param name="request">Current and new password</param>
    /// <returns>Success message</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(BaseResponse<string>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BaseResponse<string>>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return BadRequest(BaseResponse<string>.Failure("Invalid user token"));
        }

        var result = await authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
        
        if (result.IsSuccess)
            return Ok(result);
            
        return BadRequest(result);
    }
}
