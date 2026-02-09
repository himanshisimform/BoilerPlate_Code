using System.ComponentModel.DataAnnotations;

namespace sourceName.DTO;

public class LoginRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    public bool RememberMe { get; set; } = false;
}

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public UserDto User { get; set; } = null!;
}

public class RegisterRequest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required, MinLength(3)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required, MinLength(3)]
    public string LastName { get; set; } = string.Empty;
    
    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
    
    [Required, Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
    
    public string? PhoneNumber { get; set; }
}
