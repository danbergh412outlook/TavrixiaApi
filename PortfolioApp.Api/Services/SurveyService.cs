using Azure;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.Api.Data;
using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Helpers;
using PortfolioApp.Api.Models;

namespace PortfolioApp.Api.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly AppDbContext _context;
        private readonly IGoogleUserService _googleUserService;
        private readonly IAppUserService _appUserService;

        public SurveyService(AppDbContext context, IGoogleUserService googleUserService, IAppUserService appUserService)
        {
            _context = context;
            _googleUserService = googleUserService;
            _appUserService = appUserService;
        }

        public async Task<SurveyDto> GetSurveyDtoWithQuestionsByUrlNameAsync(string urlName)
        {
            var survey = await GetSurveyWithQuestionsByUrlNameAsync(urlName, false);

            if (survey == null)
                return null;

            return Mappers.SurveyToDto(survey);
        }
        public async Task<Survey?> GetSurveyWithQuestionsByUrlNameAsync(string urlName, bool includeUserSurveys)
        {
            var query = _context.Surveys.AsQueryable();

            query = query.Include(s => s.AppUser)
                    .Include(s => s.SurveyQuestions)
                    .ThenInclude(q => q.SurveyResponses);

            if (includeUserSurveys)
            {
                query = query
                    .Include(s => s.UserSurveys)
                        .ThenInclude(us => us.UserResponses);
            }

            var email = _googleUserService.GetEmail();

            return await query.FirstOrDefaultAsync(s => s.UrlName == urlName && s.AppUser.Email == email);
        }

        public async Task<List<SurveyDto>> GetAllSurveysAsync()
        {
            return await _context.Surveys
                    .Include(s => s.AppUser)
                .Where(e => e.AppUser.Email == _googleUserService.GetEmail())
                .Select(s => Mappers.SurveyToDto(s)).ToListAsync();
        }
        public async Task<HashSet<string>> GetExistingSurveyUrlNames(string baseSlug, int? surveyId)
        {
            var existingSlugs = await _context.Surveys
                .Include(s => s.AppUser)
                .Where(s => s.UrlName.StartsWith(baseSlug) && s.Id != surveyId && s.AppUser.Email == _googleUserService.GetEmail())
                .Select(s => s.UrlName)
                .ToHashSetAsync();
            return existingSlugs;
        }
        private void removeResponses(Survey survey)
        {
            foreach (var userSurvey in survey.UserSurveys.ToList())
            {
                foreach (var userResponse in userSurvey.UserResponses.ToList())
                {
                    _context.UserSurveyResponses.Remove(userResponse);
                }
                _context.UserSurveys.Remove(userSurvey);
            }
        }
        public async Task<bool> DeleteSurveyAsync(string urlName)
        {
            var survey = await GetSurveyWithQuestionsByUrlNameAsync(urlName, true);

            if (survey == null)
            {
                return false;
            }

            this.removeResponses(survey);

            foreach (var existingQuestion in survey.SurveyQuestions.ToList())
            {
                // Manually delete responses first
                foreach (var response in existingQuestion.SurveyResponses.ToList())
                {
                    _context.SurveyResponses.Remove(response);
                }

                _context.SurveyQuestions.Remove(existingQuestion);
            }
            _context.Surveys.Remove(survey);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<SurveyDto> UpdateSurveyAsync(UpdateSurveyDto dto)
        {
            bool isEditMode = dto.Id != null;
            Survey survey;

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Survey name cannot be null, empty, or whitespace.", nameof(dto.Name));

            if (isEditMode)
            {
                survey = await GetSurveyWithQuestionsByUrlNameAsync(dto.UrlName, true);

                if (survey == null)
                {
                    return null;
                }

                var uniqueSlug = await Utils.GenerateUniqueSlug(dto.Name, survey.Name, survey.UrlName, survey.Id, GetExistingSurveyUrlNames);

                survey.Name = dto.Name;
                survey.UrlName = uniqueSlug;
            }
            else
            {
                var uniqueSlug = await Utils.GenerateUniqueSlug(dto.Name, null, null, null, GetExistingSurveyUrlNames);
                var appUser = await _appUserService.GetOrCreateUser();
                survey = Mappers.CreateSurveyFromDto(dto, appUser, uniqueSlug);
                _context.Surveys.Add(survey);
            }

            // Remove deleted questions
            if (isEditMode)
            {
                this.removeResponses(survey);
                foreach (var existingQuestion in survey.SurveyQuestions.ToList())
                {
                    if (!dto.SurveyQuestions.Any(q => q.Id == existingQuestion.Id))
                    {
                        // Manually delete responses first
                        foreach (var response in existingQuestion.SurveyResponses.ToList())
                        {
                            _context.SurveyResponses.Remove(response);
                        }

                        _context.SurveyQuestions.Remove(existingQuestion);
                    }
                }
            }

            foreach (var qDto in dto.SurveyQuestions)
            {
                var question = survey.SurveyQuestions.FirstOrDefault(q => q.Id == qDto.Id);

                if (question != null)
                {
                    question.Text = qDto.Text;

                    // Remove deleted responses
                    foreach (var existingResponse in question.SurveyResponses.ToList())
                    {
                        if (!qDto.SurveyResponses.Any(r => r.Id == existingResponse.Id))
                        {
                            _context.SurveyResponses.Remove(existingResponse);
                        }
                    }

                    // Add or update responses
                    foreach (var rDto in qDto.SurveyResponses)
                    {
                        var response = question.SurveyResponses.FirstOrDefault(r => r.Id == rDto.Id);
                        if (response != null)
                        {
                            response.Text = rDto.Text;
                        }
                        else
                        {
                            question.SurveyResponses.Add(Mappers.SurveyResponseFromUpdateDto(rDto, survey, question));
                        }
                    }
                }
                else
                {
                    // New question with new responses
                    var newQuestion = Mappers.SurveyQuestionFromUpdateDto(qDto, survey);

                    survey.SurveyQuestions.Add(newQuestion);
                }
            }

            await _context.SaveChangesAsync();

            return Mappers.SurveyToDto(survey);
        }
    }
}
