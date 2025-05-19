using Microsoft.EntityFrameworkCore;
using PortfolioApp.Api.Data;
using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Helpers;
using PortfolioApp.Api.Models;

namespace PortfolioApp.Api.Services
{
    public class UserSurveyService : IUserSurveyService
    {
        private readonly AppDbContext _context;
        private readonly IGoogleUserService _googleUserService;
        private readonly IAppUserService _appUserService;
        private readonly ISurveyService _surveyService;

        public UserSurveyService(AppDbContext context, IGoogleUserService googleUserService, IAppUserService appUserService, ISurveyService surveyService)
        {
            _context = context;
            _googleUserService = googleUserService;
            _appUserService = appUserService;
            _surveyService = surveyService;
        }
        public async Task<UserSurvey?> GetUserSurveyAsync(
            string surveyUrlName,
        bool currentUser,
            string? userUrlName)
        {
            var userEmail = _googleUserService.GetEmail();

            IQueryable<UserSurvey> query = _context.UserSurveys
                .Include(s => s.Survey)
                    .ThenInclude(s => s.AppUser)
                .Include(s => s.AppUser)
                .Include(s => s.UserResponses)
                    .ThenInclude(r => r.SurveyResponse)
                        .ThenInclude(r => r.SurveyQuestion)
                .Where(s =>
                    s.Survey.UrlName == surveyUrlName &&
                    (s.AppUser.Email == userEmail || s.Survey.AppUser.Email == userEmail)
                );

            if (currentUser)
            {
                query = query.Where(s => s.AppUser.Email == userEmail);
            }
            else if (!string.IsNullOrWhiteSpace(userUrlName))
            {
                query = query.Where(s => s.AppUser.UrlName == userUrlName);
            }
            else
            {
                return null; // Not enough info
            }

            return await query.FirstOrDefaultAsync();
        }
        public async Task<UserSurveyDto> GetUserSurveyDtoAsync(string surveyUrlName, bool currentUser, string userUrlName)
        {
            var survey = await GetUserSurveyAsync(surveyUrlName, currentUser, userUrlName);

            if (survey == null)
            {
                return null;
            }

            var surveyDto = Mappers.UserSurveyToDto(survey);

            return surveyDto;
        }

        public async Task<UserSurveyDto> CreateUserSurveyAsync(CreateUserSurveyDto createUserSurveyDto)
        {
            var survey = await _surveyService.GetSurveyWithQuestionsByUrlNameAsync(createUserSurveyDto.SurveyUrlName, false);
            var currentUser = await _appUserService.GetOrCreateUser();

            if (survey == null)
                throw new Exception("Survey not found.");

            var userSurvey = Mappers.CreateUserSurveyFromDto(createUserSurveyDto, currentUser, survey);

            foreach (var response in createUserSurveyDto.UserResponses)
            {
                var question = survey.SurveyQuestions
                    .FirstOrDefault(q => q.SurveyResponses.Any(r => r.Id == response.SurveyResponseId));

                if (question == null)
                    throw new Exception($"No matching question found for response ID {response.SurveyResponseId}");

                var userResponse = Mappers.CreateUserResponseFromDto(response, survey, question);

                userSurvey.UserResponses.Add(userResponse);
                _context.UserSurveyResponses.Add(userResponse);
            }

            _context.UserSurveys.Add(userSurvey);
            await _context.SaveChangesAsync();

            return Mappers.UserSurveyToDto(userSurvey);
        }
    }
}
