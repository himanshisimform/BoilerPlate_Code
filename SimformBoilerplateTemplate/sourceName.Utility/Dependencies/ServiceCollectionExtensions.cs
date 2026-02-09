using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace sourceName.Utility.Dependencies;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddUtilityServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add utility services here
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddScoped<IPasswordGenerator, PasswordGenerator>();
        services.AddScoped<IEmailValidator, EmailValidator>();

        return services;
    }
}

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Now => DateTime.Now;
}

public interface IPasswordGenerator
{
    string GeneratePassword(int length = 12, bool includeSpecialChars = true);
    bool IsPasswordStrong(string password);
}

public class PasswordGenerator : IPasswordGenerator
{
    private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
    private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string Numbers = "0123456789";
    private const string SpecialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

    public string GeneratePassword(int length = 12, bool includeSpecialChars = true)
    {
        var chars = LowerCase + UpperCase + Numbers;
        if (includeSpecialChars)
            chars += SpecialChars;

        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public bool IsPasswordStrong(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 8)
            return false;

        var hasUpper = password.Any(char.IsUpper);
        var hasLower = password.Any(char.IsLower);
        var hasDigit = password.Any(char.IsDigit);
        var hasSpecial = password.Any(ch => SpecialChars.Contains(ch));

        return hasUpper && hasLower && hasDigit && hasSpecial;
    }
}

public interface IEmailValidator
{
    bool IsValidEmail(string email);
    string NormalizeEmail(string email);
}

public class EmailValidator : IEmailValidator
{
    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public string NormalizeEmail(string email)
    {
        return email?.Trim().ToLowerInvariant() ?? string.Empty;
    }
}
