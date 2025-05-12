using Microsoft.EntityFrameworkCore;
using PortfolioApp.Api.Data;
using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Helpers;
using PortfolioApp.Api.Models;

namespace PortfolioApp.Api.Services
{
    public class SurveyService
    {
        private readonly AppDbContext _context;
        private readonly CurrentUserService _currentUserService;

        public SurveyService(AppDbContext context, CurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }
        public async Task<SurveyDetailsDto?> GetSurveyWithQuestionsByUrlNameAsync(string urlName)
        {
            var survey = await _context.Surveys
                .Where(e => e.Creator == _currentUserService.GetEmail())
                .Include(s => s.SurveyQuestions)
                .ThenInclude(r => r.SurveyResponses)
                .FirstOrDefaultAsync(s => s.UrlName == urlName);

            if (survey == null)
                return null;

            return Mappers.SurveyToSurveyDetailsDto(survey);
        }

        public async Task<List<SurveyDto>> GetAllSurveysAsync()
        {
            return await _context.Surveys
                .Where(e => e.Creator == _currentUserService.GetEmail())
                .Select(s => Mappers.SurveyDtoToSurvey(s)).ToListAsync();
        }
        public async Task<HashSet<string>> GetExistingSurveyUrlNames(string baseSlug, int? surveyId)
        {
            var existingSlugs = await _context.Surveys
                .Where(s => s.UrlName.StartsWith(baseSlug) && s.Id != surveyId && s.Creator == _currentUserService.GetEmail())
                .Select(s => s.UrlName)
                .ToHashSetAsync();
            return existingSlugs;
        }
        public async Task<SurveyDto> UpdateSurveyAsync(UpdateSurveyDto dto)
        {
            bool isEditMode = dto.Id != null;
            Survey survey;


            if (isEditMode)
            {
                survey = await _context.Surveys
                    .Where(e => e.Creator == _currentUserService.GetEmail())
                    .Include(s => s.SurveyQuestions)
                        .ThenInclude(q => q.SurveyResponses)
                    .FirstOrDefaultAsync(s => s.Id == dto.Id.Value);

                var uniqueSlug = await Utils.GenerateUniqueSlug(dto.Name, survey, GetExistingSurveyUrlNames);

                survey.Name = dto.Name;
                survey.UrlName = uniqueSlug;
            }
            else
            {
                var uniqueSlug = await Utils.GenerateUniqueSlug(dto.Name, null, GetExistingSurveyUrlNames);
                survey = new Survey
                {
                    Name = dto.Name,
                    Creator = dto.Creator,
                    UrlName = uniqueSlug,
                    DateCreated = DateTime.UtcNow,
                    SurveyQuestions = new List<SurveyQuestion>()
                };
                _context.Surveys.Add(survey);
            }

            // Remove deleted questions
            if (isEditMode)
            {
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
                            question.SurveyResponses.Add(new SurveyResponse
                            {
                                Text = rDto.Text,
                                SurveyId = survey.Id,
                                SurveyQuestionId = question.Id
                            });
                        }
                    }
                }
                else
                {
                    // New question with new responses
                    var newQuestion = new SurveyQuestion
                    {
                        Text = qDto.Text,
                        SurveyId = survey.Id,
                        SurveyResponses = qDto.SurveyResponses.Select(r => new SurveyResponse
                        {
                            Text = r.Text,
                            Survey = survey
                        }).ToList()
                    };

                    survey.SurveyQuestions.Add(newQuestion);
                }
            }

            await _context.SaveChangesAsync();

            return Mappers.SurveyToSurveyDto(survey);
        }
    }
}
