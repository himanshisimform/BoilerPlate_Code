namespace sourceName.Service;

public static class ResponseMessage
{
    // Success Messages
    public const string Success = "Operation completed successfully";
    public const string LoginSuccess = "Login successful";
    public const string LogoutSuccess = "Logout successful";
    public const string RegisterSuccess = "Registration successful";
    public const string PasswordChangedSuccess = "Password changed successfully";
    public const string TokenRefreshedSuccess = "Token refreshed successfully";

    // Error Messages
    public const string InvalidCredentials = "Invalid email or password";
    public const string AccountLocked = "Account is locked out";
    public const string AccountNotActivated = "Account is not activated";
    public const string EmailAlreadyExists = "Email is already registered";
    public const string UserNotFound = "User not found";
    public const string InvalidRefreshToken = "Invalid refresh token";
    public const string AccountDeactivated = "User account is deactivated";
    public const string ValidationFailed = "Validation failed";

    // Generic Messages
    public const string InternalError = "An internal error occurred";
    public const string UnauthorizedAccess = "Unauthorized access";
    public const string ForbiddenAccess = "Forbidden access";
    public const string NotFound = "Resource not found";
    public const string BadRequest = "Bad request";
}
