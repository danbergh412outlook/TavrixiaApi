using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.Api.Services;
using PortfolioApp.Api.Models;
using PortfolioApp.Api.Data;

public class AppUserServiceTests
{
    private readonly AppDbContext _context;
    private readonly Mock<IGoogleUserService> _mockGoogleUser;
    private readonly IAppUserService _service;

    public AppUserServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);

        _mockGoogleUser = new Mock<IGoogleUserService>();
        _service = new AppUserService(_mockGoogleUser.Object, _context);
    }

    [Fact]
    public async Task FindUser_ReturnsUser_WhenExists()
    {
        var email = "test@example.com";
        var user = new AppUser { Email = email, Name = "Test User", UrlName = "test-user" };
        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        var result = await _service.FindUser(email);

        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.Equal("Test User", result.Name);
        Assert.Equal("test-user", result.UrlName);
    }

    [Fact]
    public async Task FindUser_ReturnsNull_WhenNotExists()
    {
        var result = await _service.FindUser("nonexistent@example.com");
        Assert.Null(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task FindUser_ReturnsNull_WhenEmailIsNullOrWhitespace(string badEmail)
    {
        var result = await _service.FindUser(badEmail);
        Assert.Null(result);
    }

    [Fact]
    public async Task FindUser_IsCaseInsensitive()
    {
        var email = "Test@Example.com";
        _context.AppUsers.Add(new AppUser { Email = email, Name = "Test User", UrlName = "test-user" });
        await _context.SaveChangesAsync();

        var result = await _service.FindUser("test@example.com");
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
    }

    [Fact]
    public async Task GetExistingUserUrlNames_ReturnsExpectedSlugs()
    {
        _context.AppUsers.AddRange(
            new AppUser { Id = 1, Email = "john1@example.com", Name = "John", UrlName = "john-doe" },
            new AppUser { Id = 2, Email = "john2@example.com", Name = "John", UrlName = "john-doe-1" },
            new AppUser { Id = 3, Email = "jane@example.com", Name = "Jane", UrlName = "jane-doe" }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetExistingUserUrlNames("john-doe", 1);

        Assert.Contains("john-doe-1", result);
        Assert.DoesNotContain("john-doe", result); // Excluded by ID match
        Assert.DoesNotContain("jane-doe", result); // Not matching baseSlug
    }

    [Fact]
    public async Task GetExistingUserUrlNames_ReturnsEmpty_WhenNoMatches()
    {
        _context.AppUsers.Add(new AppUser { Id = 1, Email = "a@example.com", Name = "A", UrlName = "a" });
        await _context.SaveChangesAsync();

        var result = await _service.GetExistingUserUrlNames("zzz", null);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetOrCreateUser_ReturnsExistingUser()
    {
        var email = "existing@example.com";
        var user = new AppUser
        {
            Email = email,
            Name = "Existing User",
            UrlName = "existing-user"
        };
        _context.AppUsers.Add(user);
        await _context.SaveChangesAsync();

        _mockGoogleUser.Setup(g => g.GetEmail()).Returns(email);
        _mockGoogleUser.Setup(g => g.GetFullName()).Returns("Existing User");

        var result = await _service.GetOrCreateUser();

        Assert.Equal(email, result.Email);
        Assert.Equal("existing-user", result.UrlName);
        Assert.Equal(1, _context.AppUsers.Count());
    }

    [Fact]
    public async Task GetOrCreateUser_CreatesNewUser_WhenNotFound()
    {
        var email = "new@example.com";
        var name = "New User";

        _mockGoogleUser.Setup(g => g.GetEmail()).Returns(email);
        _mockGoogleUser.Setup(g => g.GetFullName()).Returns(name);

        var user = await _service.GetOrCreateUser();

        Assert.Equal(email, user.Email);
        Assert.Equal(name, user.Name);
        Assert.False(string.IsNullOrEmpty(user.UrlName));
        Assert.Equal("new-user", user.UrlName);
        Assert.Equal(1, _context.AppUsers.Count());
    }

    [Fact]
    public async Task GetOrCreateUser_Throws_WhenEmailIsNull()
    {
        _mockGoogleUser.Setup(g => g.GetEmail()).Returns((string)null);
        _mockGoogleUser.Setup(g => g.GetFullName()).Returns("Name");

        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetOrCreateUser());
    }

    [Fact]
    public async Task GetOrCreateUser_Throws_WhenFullNameIsNull()
    {
        _mockGoogleUser.Setup(g => g.GetEmail()).Returns("email@example.com");
        _mockGoogleUser.Setup(g => g.GetFullName()).Returns((string)null);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetOrCreateUser());
    }

    [Fact]
    public async Task GetOrCreateUser_ReturnsSameInstance_OnMultipleCalls()
    {
        var email = "multi@example.com";
        var name = "Multi User";
        _mockGoogleUser.Setup(g => g.GetEmail()).Returns(email);
        _mockGoogleUser.Setup(g => g.GetFullName()).Returns(name);

        var user1 = await _service.GetOrCreateUser();
        var user2 = await _service.GetOrCreateUser();

        Assert.Same(user1, user2);
        Assert.Equal(1, _context.AppUsers.Count());
    }

    [Fact]
    public async Task GetOrCreateUser_RespectsUniqueUrlName()
    {
        // Simulate existing similar slugs
        _context.AppUsers.Add(new AppUser { Email = "a@a.com", Name = "A", UrlName = "john-doe" });
        _context.AppUsers.Add(new AppUser { Email = "b@b.com", Name = "B", UrlName = "john-doe-1" });
        await _context.SaveChangesAsync();

        _mockGoogleUser.Setup(g => g.GetEmail()).Returns("john@example.com");
        _mockGoogleUser.Setup(g => g.GetFullName()).Returns("John Doe");

        var user = await _service.GetOrCreateUser();

        Assert.StartsWith("john-doe", user.UrlName);
        Assert.DoesNotContain(_context.AppUsers.Where(u => u.Email != user.Email), u => u.UrlName == user.UrlName);
    }
}