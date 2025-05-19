using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Models;

namespace PortfolioApp.Api.Services
{
    public interface IUserSurveyService
    {
        Task<UserSurveyDto> CreateUserSurveyAsync(CreateUserSurveyDto createUserSurveyDto);
        Task<UserSurvey?> GetUserSurveyAsync(string surveyUrlName, bool currentUser, string? userUrlName);
        Task<UserSurveyDto> GetUserSurveyDtoAsync(string surveyUrlName, bool currentUser, string userUrlName);
    }
}