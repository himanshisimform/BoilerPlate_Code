using Xunit;
using FluentAssertions;
using SimformMicroservice_Project1.Service;

namespace SimformMicroservice_Project1.UnitTest;

public class UnitTest1
{
    [Fact]
    public void ExtensionMethods_ToTitleCase_ShouldReturnCorrectFormat()
    {
        // Arrange
        var input = "hello world";

        // Act
        var result = input.ToTitleCase();

        // Assert
        result.Should().Be("Hello world");
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("invalid-email", false)]
    [InlineData("", false)]
    public void ExtensionMethods_IsValidEmail_ShouldValidateCorrectly(string email, bool expected)
    {
        // Act
        var result = email.IsValidEmail();

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void ExtensionMethods_MaskEmail_ShouldMaskCorrectly()
    {
        // Arrange
        var email = "john.doe@example.com";

        // Act
        var result = email.MaskEmail();

        // Assert
        result.Should().Be("j*****e@example.com");
    }

    [Fact]
    public void ExtensionMethods_GenerateRandomString_ShouldReturnCorrectLength()
    {
        // Arrange
        var length = 10;

        // Act
        var result = ExtensionMethods.GenerateRandomString(length);

        // Assert
        result.Should().HaveLength(length);
        result.Should().MatchRegex("^[A-Z0-9]+$");
    }
}