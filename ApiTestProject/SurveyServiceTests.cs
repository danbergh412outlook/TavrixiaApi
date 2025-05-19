using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using PortfolioApp.Api.Data;
using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Helpers;
using PortfolioApp.Api.Models;
using PortfolioApp.Api.Services;
using Xunit;

public class SurveyServiceTests
{
    private AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    private SurveyService CreateService(
        AppDbContext db,
        string userEmail = "test@example.com",
        IAppUserService appUserService = null)
    {
        var googleUserService = new Mock<IGoogleUserService>();
        googleUserService.Setup(x => x.GetEmail()).Returns(userEmail);

        return new SurveyService(
            db,
            googleUserService.Object,
            appUserService ?? new Mock<IAppUserService>().Object
        );
    }

    [Fact]
    public async Task GetSurveyDtoWithQuestionsByUrlNameAsync_ReturnsNull_WhenSurveyNotFound()
    {
        var db = GetDbContext();
        var service = CreateService(db);

        var result = await service.GetSurveyDtoWithQuestionsByUrlNameAsync("notfound");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetSurveyDtoWithQuestionsByUrlNameAsync_ReturnsDto_WithAllProperties()
    {
        var db = GetDbContext();
        var user = new AppUser { Id = 1, Email = "test@example.com", Name = "Test", UrlName = "test" };
        var question = new SurveyQuestion
        {
            Id = 1,
            SurveyId = 1,
            Text = "Q1",
            SurveyResponses = new List<SurveyResponse>
            {
                new SurveyResponse { Id = 1, SurveyId = 1, SurveyQuestionId = 1, Text = "A1" }
            }
        };
        var survey = new Survey
        {
            Id = 1,
            AppUserId = 1,
            AppUser = user,
            Name = "Survey1",
            UrlName = "survey1",
            DateCreated = DateTime.UtcNow,
            SurveyQuestions = new List<SurveyQuestion> { question }
        };
        db.AppUsers.Add(user);
        db.Surveys.Add(survey);
        db.SaveChanges();

        var service = CreateService(db);

        var result = await service.GetSurveyDtoWithQuestionsByUrlNameAsync("survey1");

        Assert.NotNull(result);
        Assert.Equal("Survey1", result.Name);
        Assert.Equal("survey1", result.UrlName);
        Assert.Equal(user.Email, result.CreatorEmail);
        Assert.Equal(user.Name, result.CreatorName);
        Assert.NotNull(result.SurveyQuestions);
        Assert.Single(result.SurveyQuestions);
        Assert.Equal("Q1", result.SurveyQuestions[0].Text);
        Assert.Single(result.SurveyQuestions[0].SurveyResponses);
        Assert.Equal("A1", result.SurveyQuestions[0].SurveyResponses[0].Text);
    }

    [Fact]
    public async Task GetSurveyWithQuestionsByUrlNameAsync_ReturnsNull_WhenNotFoundOrWrongUser()
    {
        var db = GetDbContext();
        var user = new AppUser { Id = 1, Email = "other@example.com", Name = "Other", UrlName = "other" };
        db.AppUsers.Add(user);
        db.Surveys.Add(new Survey
        {
            Id = 1,
            AppUserId = 1,
            AppUser = user,
            Name = "Survey1",
            UrlName = "survey1",
            DateCreated = DateTime.UtcNow,
            SurveyQuestions = new List<SurveyQuestion>()
        });
        db.SaveChanges();

        var service = CreateService(db, "test@example.com");

        var result = await service.GetSurveyWithQuestionsByUrlNameAsync("survey1", false);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetSurveyWithQuestionsByUrlNameAsync_ReturnsSurvey_WhenExistsAndUserMatches()
    {
        var db = GetDbContext();
        var user = new AppUser { Id = 1, Email = "test@example.com", Name = "Test", UrlName = "test" };
        db.AppUsers.Add(user);
        db.Surveys.Add(new Survey
        {
            Id = 1,
            AppUserId = 1,
            AppUser = user,
            Name = "Survey1",
            UrlName = "survey1",
            DateCreated = DateTime.UtcNow,
            SurveyQuestions = new List<SurveyQuestion>()
        });
        db.SaveChanges();

        var service = CreateService(db);

        var result = await service.GetSurveyWithQuestionsByUrlNameAsync("survey1", false);

        Assert.NotNull(result);
        Assert.Equal("Survey1", result.Name);
    }

    [Fact]
    public async Task GetAllSurveysAsync_ReturnsOnlyCurrentUserSurveys()
    {
        var db = GetDbContext();
        db.AppUsers.Add(new AppUser { Id = 1, Email = "test@example.com", Name = "Test", UrlName = "test" });
        db.AppUsers.Add(new AppUser { Id = 2, Email = "other@example.com", Name = "Other", UrlName = "other" });
        db.Surveys.Add(new Survey
        {
            Id = 1,
            AppUserId = 1,
            Name = "Survey1",
            UrlName = "survey1",
            DateCreated = DateTime.UtcNow,
            AppUser = db.AppUsers.Find(1)
        });
        db.Surveys.Add(new Survey
        {
            Id = 2,
            AppUserId = 2,
            Name = "Survey2",
            UrlName = "survey2",
            DateCreated = DateTime.UtcNow,
            AppUser = db.AppUsers.Find(2)
        });
        db.SaveChanges();

        var service = CreateService(db);

        var result = await service.GetAllSurveysAsync();

        Assert.Single(result);
        Assert.Equal("Survey1", result[0].Name);
    }

    [Fact]
    public async Task GetExistingSurveyUrlNames_ReturnsMatchingSlugs()
    {
        var db = GetDbContext();
        db.AppUsers.Add(new AppUser { Id = 1, Email = "test@example.com", Name = "Test", UrlName = "test" });
        db.Surveys.Add(new Survey
        {
            Id = 1,
            AppUserId = 1,
            Name = "Survey1",
            UrlName = "slug-1",
            DateCreated = DateTime.UtcNow,
            AppUser = db.AppUsers.Find(1)
        });
        db.Surveys.Add(new Survey
        {
            Id = 2,
            AppUserId = 1,
            Name = "Survey2",
            UrlName = "slug-2",
            DateCreated = DateTime.UtcNow,
            AppUser = db.AppUsers.Find(1)
        });
        db.SaveChanges();

        var service = CreateService(db);

        var result = await service.GetExistingSurveyUrlNames("slug", null);

        Assert.Contains("slug-1", result);
        Assert.Contains("slug-2", result);
    }

    [Fact]
    public async Task DeleteSurveyAsync_ReturnsFalse_WhenSurveyNotFound()
    {
        var db = GetDbContext();
        var service = CreateService(db);

        var result = await service.DeleteSurveyAsync("notfound");

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteSurveyAsync_RemovesSurveyAndRelatedEntities()
    {
        var db = GetDbContext();
        var user = new AppUser { Id = 1, Email = "test@example.com", Name = "Test", UrlName = "test" };
        var question = new SurveyQuestion
        {
            Id = 1,
            SurveyId = 1,
            Text = "Q1",
            SurveyResponses = new List<SurveyResponse>
            {
                new SurveyResponse { Id = 1, SurveyId = 1, SurveyQuestionId = 1, Text = "A1" }
            }
        };
        var survey = new Survey
        {
            Id = 1,
            AppUserId = 1,
            AppUser = user,
            Name = "Survey1",
            UrlName = "survey1",
            DateCreated = DateTime.UtcNow,
            SurveyQuestions = new List<SurveyQuestion> { question },
            UserSurveys = new List<UserSurvey>()
        };
        db.AppUsers.Add(user);
        db.Surveys.Add(survey);
        db.SurveyQuestions.Add(question);
        db.SurveyResponses.AddRange(question.SurveyResponses);
        db.SaveChanges();

        var service = CreateService(db);

        var result = await service.DeleteSurveyAsync("survey1");

        Assert.True(result);
        Assert.Empty(db.Surveys);
        Assert.Empty(db.SurveyQuestions);
        Assert.Empty(db.SurveyResponses);
    }

    [Fact]
    public async Task UpdateSurveyAsync_ReturnsNull_WhenEditModeAndSurveyNotFound()
    {
        var db = GetDbContext();
        var service = CreateService(db);

        var dto = new UpdateSurveyDto
        {
            Id = 1,
            UrlName = "notfound",
            Name = "Updated",
            SurveyQuestions = new List<UpdateSurveyQuestionDto>()
        };

        var result = await service.UpdateSurveyAsync(dto);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateSurveyAsync_CreatesNewSurvey_WhenNotEditMode()
    {
        var db = GetDbContext();
        var appUser = new AppUser { Id = 1, Email = "test@example.com", Name = "Test", UrlName = "test" };
        db.AppUsers.Add(appUser);
        db.SaveChanges();

        var appUserService = new Mock<IAppUserService>();
        appUserService.Setup(x => x.GetOrCreateUser()).ReturnsAsync(appUser);

        var service = CreateService(db, appUserService: appUserService.Object);

        var dto = new UpdateSurveyDto
        {
            Name = "New Survey",
            SurveyQuestions = new List<UpdateSurveyQuestionDto>()
        };

        var result = await service.UpdateSurveyAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("New Survey", result.Name);
        Assert.Single(db.Surveys);
    }

    [Fact]
    public async Task UpdateSurveyAsync_UpdatesSurvey_WhenEditMode()
    {
        var db = GetDbContext();
        var appUser = new AppUser { Id = 1, Email = "test@example.com", Name = "Test", UrlName = "test" };
        var survey = new Survey
        {
            Id = 1,
            AppUserId = 1,
            AppUser = appUser,
            Name = "Old Survey",
            UrlName = "survey1",
            DateCreated = DateTime.UtcNow,
            SurveyQuestions = new List<SurveyQuestion>()
        };
        db.AppUsers.Add(appUser);
        db.Surveys.Add(survey);
        db.SaveChanges();

        var appUserService = new Mock<IAppUserService>();
        appUserService.Setup(x => x.GetOrCreateUser()).ReturnsAsync(appUser);

        var service = CreateService(db, appUserService: appUserService.Object);

        var dto = new UpdateSurveyDto
        {
            Id = 1,
            UrlName = "survey1",
            Name = "Updated Survey",
            SurveyQuestions = new List<UpdateSurveyQuestionDto>()
        };

        var result = await service.UpdateSurveyAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Updated Survey", result.Name);
        Assert.Single(db.Surveys);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task UpdateSurveyAsync_ThrowsException_WhenNameIsInvalid(string invalidName)
    {
        var db = GetDbContext();
        var appUser = new AppUser { Id = 1, Email = "test@example.com", Name = "Test", UrlName = "test" };
        db.AppUsers.Add(appUser);
        db.SaveChanges();

        var appUserService = new Mock<IAppUserService>();
        appUserService.Setup(x => x.GetOrCreateUser()).ReturnsAsync(appUser);

        var service = CreateService(db, appUserService: appUserService.Object);

        var dto = new UpdateSurveyDto
        {
            Name = invalidName,
            SurveyQuestions = new List<UpdateSurveyQuestionDto>()
        };

        await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateSurveyAsync(dto));
    }
}