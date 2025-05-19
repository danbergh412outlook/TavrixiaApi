using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Models;

namespace PortfolioApp.Api.Services
{
    public interface ISurveyService
    {
        Task<bool> DeleteSurveyAsync(string urlName);
        Task<List<SurveyDto>> GetAllSurveysAsync();
        Task<HashSet<string>> GetExistingSurveyUrlNames(string baseSlug, int? surveyId);
        Task<SurveyDto> GetSurveyDtoWithQuestionsByUrlNameAsync(string urlName);
        Task<Survey?> GetSurveyWithQuestionsByUrlNameAsync(string urlName, bool includeUserSurveys);
        Task<SurveyDto> UpdateSurveyAsync(UpdateSurveyDto dto);
    }
}