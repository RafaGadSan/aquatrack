using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using AquaTrack.Domain.Exceptions;
using Xunit;

namespace AquaTrack.Domain.Tests;

public class UserTests
{
    [Fact]
    public void Constructor_NormalizesEmail_ToTrimmedLowercase()
    {
        var user = new User("  Rafael@Example.com  ", "hash", "Rafael Gadea", Role.Admin);

        Assert.Equal("rafael@example.com", user.Email);
    }

    [Fact]
    public void Constructor_StartsActive()
    {
        var user = new User("rafael@example.com", "hash", "Rafael Gadea", Role.Operator);

        Assert.True(user.IsActive);
    }

    [Theory]
    [InlineData("", "hash", "Rafael")]
    [InlineData("rafael@example.com", "", "Rafael")]
    [InlineData("rafael@example.com", "hash", "")]
    public void Constructor_Throws_WhenRequiredFieldIsMissing(string email, string passwordHash, string fullName)
    {
        Assert.Throws<DomainException>(() => new User(email, passwordHash, fullName, Role.Operator));
    }

    [Fact]
    public void Deactivate_SetsIsActiveToFalse()
    {
        var user = new User("rafael@example.com", "hash", "Rafael Gadea", Role.ShiftLead);

        user.Deactivate();

        Assert.False(user.IsActive);
    }
}
