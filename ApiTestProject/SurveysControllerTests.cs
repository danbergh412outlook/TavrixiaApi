using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PortfolioApp.Api.Controllers;
using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Services;
using Xunit;

public class SurveysControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOk_WithSurveys()
    {
        // Arrange
        var mockService = new Mock<ISurveyService>();
        var expected = new List<SurveyDto>
        {
            new SurveyDto { Id = 1, Name = "Survey1", UrlName = "survey1" },
            new SurveyDto { Id = 2, Name = "Survey2", UrlName = "survey2" }
        };
        mockService.Setup(s => s.GetAllSurveysAsync()).ReturnsAsync(expected);

        var controller = new SurveysController(mockService.Object);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task GetSurveyByUrlName_ReturnsOk_WhenFound()
    {
        // Arrange
        var mockService = new Mock<ISurveyService>();
        var expected = new SurveyDto { Id = 1, Name = "Survey1", UrlName = "survey1" };
        mockService.Setup(s => s.GetSurveyDtoWithQuestionsByUrlNameAsync("survey1")).ReturnsAsync(expected);

        var controller = new SurveysController(mockService.Object);

        // Act
        var result = await controller.GetSurveyByUrlName("survey1");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(expected, okResult.Value);
    }

    [Fact]
    public async Task GetSurveyByUrlName_ReturnsNotFound_WhenNull()
    {
        // Arrange
        var mockService = new Mock<ISurveyService>();
        mockService.Setup(s => s.GetSurveyDtoWithQuestionsByUrlNameAsync("survey1")).ReturnsAsync((SurveyDto)null);

        var controller = new SurveysController(mockService.Object);

        // Act
        var result = await controller.GetSurveyByUrlName("survey1");

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithSurvey()
    {
        // Arrange
        var mockService = new Mock<ISurveyService>();
        var inputDto = new UpdateSurveyDto { Name = "Survey1" };
        var createdDto = new SurveyDto { Id = 1, Name = "Survey1", UrlName = "survey1" };
        mockService.Setup(s => s.UpdateSurveyAsync(inputDto)).ReturnsAsync(createdDto);

        var controller = new SurveysController(mockService.Object);

        // Act
        var result = await controller.Create(inputDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(createdDto, createdResult.Value);
        Assert.Equal(nameof(controller.GetAll), createdResult.ActionName);
    }

    [Fact]
    public async Task UpdateSurvey_ReturnsOk_WhenFound()
    {
        // Arrange
        var mockService = new Mock<ISurveyService>();
        var inputDto = new UpdateSurveyDto { Name = "Survey1" };
        var updatedDto = new SurveyDto { Id = 1, Name = "Survey1", UrlName = "survey1" };
        mockService.Setup(s => s.UpdateSurveyAsync(inputDto)).ReturnsAsync(updatedDto);

        var controller = new SurveysController(mockService.Object);

        // Act
        var result = await controller.UpdateSurvey("survey1", inputDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updatedDto, okResult.Value);
    }

    [Fact]
    public async Task UpdateSurvey_ReturnsNotFound_WhenNull()
    {
        // Arrange
        var mockService = new Mock<ISurveyService>();
        var inputDto = new UpdateSurveyDto { Name = "Survey1" };
        mockService.Setup(s => s.UpdateSurveyAsync(inputDto)).ReturnsAsync((SurveyDto)null);

        var controller = new SurveysController(mockService.Object);

        // Act
        var result = await controller.UpdateSurvey("survey1", inputDto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteItem_ReturnsNoContent_WhenSuccess()
    {
        // Arrange
        var mockService = new Mock<ISurveyService>();
        mockService.Setup(s => s.DeleteSurveyAsync("survey1")).ReturnsAsync(true);

        var controller = new SurveysController(mockService.Object);

        // Act
        var result = await controller.DeleteItem("survey1");

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteItem_ReturnsNotFound_WhenNotSuccess()
    {
        // Arrange
        var mockService = new Mock<ISurveyService>();
        mockService.Setup(s => s.DeleteSurveyAsync("survey1")).ReturnsAsync(false);

        var controller = new SurveysController(mockService.Object);

        // Act
        var result = await controller.DeleteItem("survey1");

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}