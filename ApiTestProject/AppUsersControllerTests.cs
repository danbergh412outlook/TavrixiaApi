using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortfolioApp.Api.Controllers;
using PortfolioApp.Api.Models;
using PortfolioApp.Api.Services;
using Xunit;

public class AppUsersControllerTests
{
    [Fact]
    public async Task Create_ReturnsOk_WithUser()
    {
        // Arrange
        var mockService = new Mock<IAppUserService>();
        var expectedUser = new AppUser
        {
            Id = 1,
            Email = "test@example.com",
            Name = "Test User",
            UrlName = "test-user"
        };
        mockService.Setup(s => s.GetOrCreateUser()).ReturnsAsync(expectedUser);

        var controller = new AppUsersController(mockService.Object);

        // Act
        var result = await controller.Create();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUser = Assert.IsType<AppUser>(okResult.Value);
        Assert.Equal(expectedUser.Id, returnedUser.Id);
        Assert.Equal(expectedUser.Email, returnedUser.Email);
        Assert.Equal(expectedUser.Name, returnedUser.Name);
        Assert.Equal(expectedUser.UrlName, returnedUser.UrlName);
        mockService.Verify(s => s.GetOrCreateUser(), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsOk_WithNull_WhenServiceReturnsNull()
    {
        // Arrange
        var mockService = new Mock<IAppUserService>();
        mockService.Setup(s => s.GetOrCreateUser()).ReturnsAsync((AppUser)null);

        var controller = new AppUsersController(mockService.Object);

        // Act
        var result = await controller.Create();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Null(okResult.Value);
        mockService.Verify(s => s.GetOrCreateUser(), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsStatusCode500_WhenServiceThrows()
    {
        // Arrange
        var mockService = new Mock<IAppUserService>();
        mockService.Setup(s => s.GetOrCreateUser()).ThrowsAsync(new System.Exception("Service failure"));

        var controller = new AppUsersController(mockService.Object);

        // Act
        var result = await Record.ExceptionAsync(() => controller.Create());

        // Assert
        Assert.NotNull(result);
        Assert.IsType<System.Exception>(result);
        mockService.Verify(s => s.GetOrCreateUser(), Times.Once);
    }

    [Fact]
    public async Task Create_CallsGetOrCreateUser_Once_NoExtraCalls()
    {
        // Arrange
        var mockService = new Mock<IAppUserService>();
        mockService.Setup(s => s.GetOrCreateUser()).ReturnsAsync(new AppUser());
        var controller = new AppUsersController(mockService.Object);

        // Act
        await controller.Create();

        // Assert
        mockService.Verify(s => s.GetOrCreateUser(), Times.Once);
        mockService.VerifyNoOtherCalls();
    }
}