using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using PortfolioApp.Api.Data;
using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Models;
using PortfolioApp.Api.Services;
using Xunit;

public class UserSurveyServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private UserSurveyService CreateService(
        AppDbContext db,
        string userEmail = "test@example.com",
        IAppUserService appUserService = null,
        ISurveyService surveyService = null)
    {
        var googleUserService = new Mock<IGoogleUserService>();
        googleUserService.Setup(x => x.GetEmail()).Returns(userEmail);

        return new UserSurveyService(
            db,
            googleUserService.Object,
            appUserService ?? new Mock<IAppUserService>().Object,
            surveyService ?? new Mock<ISurveyService>().Object
        );
    }

    [Fact]
    public async Task GetUserSurveyAsync_ReturnsNull_WhenNotEnoughInfo()
    {
        var db = GetDbContext();
        var service = CreateService(db);

        var result = await service.GetUserSurveyAsync("survey1", false, null);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserSurveyAsync_ReturnsNull_WhenNoMatch()
    {
        var db = GetDbContext();
        var user = new AppUser { Id = 1, Email = "other@example.com", Name = "Other", UrlName = "other" };
        db.AppUsers.Add(user);
        db.SaveChanges();

        var service = CreateService(db);

        var result = await service.GetUserSurveyAsync("survey1", true, null);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserSurveyAsync_ReturnsSurvey_WhenCurrentUser()
    {
        var db = GetDbContext();
        var user = new AppUser { Id = 1, Email = "test@example.com", Name = "Test", UrlName = "test" };
        var survey = new Survey { Id = 1, Name = "Survey1", UrlName = "survey1", AppUserId = 1, AppUser = user, DateCreated = DateTime.UtcNow };
        var userSurvey = new UserSurvey
        {
            Id = 1,
            AppUserId = 1,
            SurveyId = 1,
            AppUser = user,
            Survey = survey,
            DateTaken = DateTime.UtcNow,
            UserResponses = new List<UserResponse>()
        };
        db.AppUsers.Add(user);
        db.Surveys.Add(survey);
        db.UserSurveys.Add(userSurvey);
        db.SaveChanges();

        var service = CreateService(db);

        var result = await service.GetUserSurveyAsync("survey1", true, null);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetUserSurveyAsync_ReturnsSurvey_WhenUserUrlName()
    {
        var db = GetDbContext();
        var user = new AppUser { Id = 1, Email = "test@example.com", Name = "Test", UrlName = "test" };
        var survey = new Survey { Id = 1, Name = "Survey1", UrlName = "survey1", AppUserId = 1, AppUser = user, DateCreated = DateTime.UtcNow };
        var userSurvey = new UserSurvey
        {
            Id = 1,
            AppUserId = 1,
            SurveyId = 1,
            AppUser = user,
            Survey = survey,
            DateTaken = DateTime.UtcNow,
            UserResponses = new List<UserResponse>()
        };
        db.AppUsers.Add(user);
        db.Surveys.Add(survey);
        db.UserSurveys.Add(userSurvey);
        db.SaveChanges();

        var service = CreateService(db);

        var result = await service.GetUserSurveyAsync("survey1", false, "test");

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetUserSurveyDtoAsync_ReturnsNull_WhenSurveyNotFound()
    {
        var db = GetDbContext();
        var service = CreateService(db);

        var result = await service.GetUserSurveyDtoAsync("survey1", true, null);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetUserSurveyDtoAsync_ReturnsDto_WhenSurveyFound()
    {
        var db = GetDbContext();
        var user = new AppUser { Id = 1, Email = "test@example.com", Name = "Test", UrlName = "test" };
        var survey = new Survey { Id = 1, Name = "Survey1", UrlName = "survey1", AppUserId = 1, AppUser = user, DateCreated = DateTime.UtcNow };
        var userSurvey = new UserSurvey
        {
            Id = 1,
            AppUserId = 1,
            SurveyId = 1,
            AppUser = user,
            Survey = survey,
            DateTaken = DateTime.UtcNow,
            UserResponses = new List<UserResponse>()
        };
        db.AppUsers.Add(user);
        db.Surveys.Add(survey);
        db.UserSurveys.Add(userSurvey);
        db.SaveChanges();

        var service = CreateService(db);

        var result = await service.GetUserSurveyDtoAsync("survey1", true, null);

        Assert.NotNull(result);
        Assert.Equal("test", result.UserUrlName);
    }

    [Fact]
    public async Task CreateUserSurveyAsync_ThrowsException_WhenSurveyNotFound()
    {
        var db = GetDbContext();
        var appUserService = new Mock<IAppUserService>();
        var surveyService = new Mock<ISurveyService>();
        surveyService.Setup(s => s.GetSurveyWithQuestionsByUrlNameAsync(It.IsAny<string>(), false))
            .ReturnsAsync((Survey)null);

        var service = CreateService(db, appUserService: appUserService.Object, surveyService: surveyService.Object);

        var dto = new CreateUserSurveyDto
        {
            SurveyUrlName = "notfound",
            UserResponses = new List<CreateUserResponseDto>()
        };

        await Assert.ThrowsAsync<Exception>(() => service.CreateUserSurveyAsync(dto));
    }

    [Fact]
    public async Task CreateUserSurveyAsync_ThrowsException_WhenNoMatchingQuestion()
    {
        var db = GetDbContext();
        var user = new AppUser { Id = 1, Email = "test@example.com", Name = "Test", UrlName = "test" };
        var survey = new Survey
        {
            Id = 1,
            Name = "Survey1",
            UrlName = "survey1",
            AppUserId = 1,
            AppUser = user,
            DateCreated = DateTime.UtcNow,
            SurveyQuestions = new List<SurveyQuestion>
            {
                new SurveyQuestion
                {
                    Id = 1,
                    SurveyId = 1,
                    Text = "Q1",
                    SurveyResponses = new List<SurveyResponse>
                    {
                        new SurveyResponse { Id = 1, SurveyId = 1, SurveyQuestionId = 1, Text = "A1" }
                    }
                }
            }
        };

        var appUserService = new Mock<IAppUserService>();
        appUserService.Setup(x => x.GetOrCreateUser()).ReturnsAsync(user);

        var surveyService = new Mock<ISurveyService>();
        surveyService.Setup(s => s.GetSurveyWithQuestionsByUrlNameAsync("survey1", false))
            .ReturnsAsync(survey);

        var service = CreateService(db, appUserService: appUserService.Object, surveyService: surveyService.Object);

        var dto = new CreateUserSurveyDto
        {
            SurveyUrlName = "survey1",
            UserResponses = new List<CreateUserResponseDto>
            {
                new CreateUserResponseDto { SurveyQuestionId = 2, SurveyResponseId = 99 }
            }
        };

        await Assert.ThrowsAsync<Exception>(() => service.CreateUserSurveyAsync(dto));
    }

    [Fact]
    public async Task CreateUserSurveyAsync_CreatesSurveyAndResponses()
    {
        var db = GetDbContext();
        var user = new AppUser { Id = 1, Email = "test@example.com", Name = "Test", UrlName = "test" };
        var surveyQuestion = new SurveyQuestion
        {
            Id = 1,
            SurveyId = 1,
            Text = "Q1",
            SurveyResponses = new List<SurveyResponse>()
        };
        var surveyResponse = new SurveyResponse
        {
            Id = 1,
            SurveyId = 1,
            SurveyQuestionId = 1,
            Text = "A1",
            SurveyQuestion = surveyQuestion
        };
        surveyQuestion.SurveyResponses.Add(surveyResponse);

        var survey = new Survey
        {
            Id = 1,
            Name = "Survey1",
            UrlName = "survey1",
            AppUserId = 1,
            AppUser = user,
            DateCreated = DateTime.UtcNow,
            SurveyQuestions = new List<SurveyQuestion> { surveyQuestion }
        };

        var appUserService = new Mock<IAppUserService>();
        appUserService.Setup(x => x.GetOrCreateUser()).ReturnsAsync(user);

        var surveyService = new Mock<ISurveyService>();
        surveyService.Setup(s => s.GetSurveyWithQuestionsByUrlNameAsync("survey1", false))
            .ReturnsAsync(survey);

        var service = CreateService(db, appUserService: appUserService.Object, surveyService: surveyService.Object);

        var dto = new CreateUserSurveyDto
        {
            SurveyUrlName = "survey1",
            UserResponses = new List<CreateUserResponseDto>
        {
            new CreateUserResponseDto { SurveyQuestionId = 1, SurveyResponseId = 1 }
        }
        };

        var result = await service.CreateUserSurveyAsync(dto);

        // Ensure navigation properties are set for assertions/mapping
        var createdUserSurvey = db.UserSurveys
            .Include(us => us.AppUser)
            .Include(us => us.Survey)
            .Include(us => us.UserResponses)
                .ThenInclude(ur => ur.SurveyQuestion)
            .Include(us => us.UserResponses)
                .ThenInclude(ur => ur.SurveyResponse)
            .First();

        Assert.NotNull(result);
        Assert.Equal("test", result.UserUrlName);
        Assert.Single(db.UserSurveys);
        Assert.Equal("Q1", createdUserSurvey.UserResponses.First().SurveyQuestion.Text);
        Assert.Equal("A1", createdUserSurvey.UserResponses.First().SurveyResponse.Text);
    }
}