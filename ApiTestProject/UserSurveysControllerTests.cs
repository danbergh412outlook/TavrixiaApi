using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortfolioApp.Api.Controllers;
using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Services;
using Xunit;

public class UserSurveysControllerTests
{
    [Fact]
    public async Task Create_ReturnsOk_WithId()
    {
        // Arrange
        var mockService = new Mock<IUserSurveyService>();
        var expectedDto = new UserSurveyDto { UserUrlName = "user", SurveyName = "survey", SurveyUrlName = "survey-url" };
        mockService.Setup(s => s.CreateUserSurveyAsync(It.IsAny<CreateUserSurveyDto>()))
            .ReturnsAsync(expectedDto);

        var controller = new UserSurveysController(mockService.Object);
        var inputDto = new CreateUserSurveyDto { SurveyUrlName = "survey-url", UserResponses = new() };

        // Act
        var result = await controller.Create(inputDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(expectedDto, okResult.Value);
    }

    [Fact]
    public async Task GetSurveyWithUserInfo_ReturnsOk_WithResult()
    {
        // Arrange
        var mockService = new Mock<IUserSurveyService>();
        var expectedDto = new UserSurveyDto { UserUrlName = "user", SurveyName = "survey", SurveyUrlName = "survey-url" };
        mockService.Setup(s => s.GetUserSurveyDtoAsync("survey-url", true, "user"))
            .ReturnsAsync(expectedDto);

        var controller = new UserSurveysController(mockService.Object);

        // Act
        var result = await controller.GetSurveyWithUserInfo("survey-url", errorNotFound: false, currentUser: true, userUrlName: "user");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expectedDto, okResult.Value);
    }

    [Fact]
    public async Task GetSurveyWithUserInfo_ReturnsOk_WithNull_WhenResultIsNull_AndErrorNotFoundFalse()
    {
        // Arrange
        var mockService = new Mock<IUserSurveyService>();
        mockService.Setup(s => s.GetUserSurveyDtoAsync("survey-url", false, null))
            .ReturnsAsync((UserSurveyDto)null);

        var controller = new UserSurveysController(mockService.Object);

        // Act
        var result = await controller.GetSurveyWithUserInfo("survey-url", errorNotFound: false, currentUser: false, userUrlName: null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Null(okResult.Value);
    }

    [Fact]
    public async Task GetSurveyWithUserInfo_ReturnsNotFound_WhenResultIsNull_AndErrorNotFoundTrue()
    {
        // Arrange
        var mockService = new Mock<IUserSurveyService>();
        mockService.Setup(s => s.GetUserSurveyDtoAsync("survey-url", false, null))
            .ReturnsAsync((UserSurveyDto)null);

        var controller = new UserSurveysController(mockService.Object);

        // Act
        var result = await controller.GetSurveyWithUserInfo("survey-url", errorNotFound: true, currentUser: false, userUrlName: null);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }
}