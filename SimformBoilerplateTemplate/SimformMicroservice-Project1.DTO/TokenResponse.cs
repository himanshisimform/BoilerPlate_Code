namespace SimformMicroservice_Project1.DTO;

public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; } = 3600;
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}