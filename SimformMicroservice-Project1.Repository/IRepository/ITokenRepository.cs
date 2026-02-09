using SimformMicroservice_Project1.Database.Tables;

namespace SimformMicroservice_Project1.Repository.IRepository;

public interface ITokenRepository
{
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task<RefreshToken> SaveRefreshTokenAsync(RefreshToken refreshToken);
    Task RevokeRefreshTokenAsync(string token, string revokedByIp, string? reason = null);
    Task RevokeAllUserTokensAsync(Guid userId, string revokedByIp, string? reason = null);
    Task<IEnumerable<RefreshToken>> GetUserActiveTokensAsync(Guid userId);
    Task CleanupExpiredTokensAsync();
}