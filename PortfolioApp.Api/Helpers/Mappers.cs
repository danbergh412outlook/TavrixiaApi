using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Models;

namespace PortfolioApp.Api.Helpers
{
    public class Mappers
    {
        public static Survey CreateSurveyFromDto(UpdateSurveyDto surveyDto, AppUser appUser, string uniqueSlug)
        {
            return new Survey
            {
                Name = surveyDto.Name,
                AppUserId = appUser.Id,
                UrlName = uniqueSlug,
                DateCreated = DateTime.UtcNow,
                AppUser = appUser,
                AllowEmulation = true,
                SurveyStatusId = 2,
                SurveyQuestions = new List<SurveyQuestion>()
            };
        }
        public static UserSurvey CreateUserSurveyFromDto(CreateUserSurveyDto dto, AppUser appUser, Survey survey)
        {
            var userSurvey = new UserSurvey
            {
                DateTaken = DateTime.UtcNow,
                SurveyId = survey.Id,
                AppUserId = appUser.Id,
                IsEmulation = false,
                ResponseStatusId = 3,
                AppUser = appUser,
                Survey = survey,
                UserResponses = new List<UserResponse>()
            };

            return userSurvey;
        }
        public static SurveyQuestionDto SurveyQuestionToDto(SurveyQuestion question)
        {
            return new SurveyQuestionDto
            {
                Text = question.Text,
                Id = question.Id,
                SurveyId = question.SurveyId,
                SurveyResponses = question.SurveyResponses.Select(r => SurveyResponseToDto(r)).ToList()
            };
        }
        public static SurveyResponseDto SurveyResponseToDto(SurveyResponse question)
        {
            return new SurveyResponseDto
            {
                Text = question.Text,
                Id = question.Id,
                SurveyId = question.SurveyId,
                SurveyQuestionId = question.SurveyQuestionId
            };
        }
        public static UserResponse CreateUserResponseFromDto(CreateUserResponseDto createResponseDto, Survey survey, SurveyQuestion question)
        {
            var userResponse = new UserResponse
            {
                SurveyId = survey.Id,
                SurveyResponseId = createResponseDto.SurveyResponseId,
                SurveyQuestionId = question.Id
            };

            return userResponse;
        }
        public static SurveyDto SurveyToDto(Survey survey)
        {
            return new SurveyDto
            {
                Id = survey.Id,
                Name = survey.Name,
                UrlName = survey.UrlName,
                CreatorEmail = survey.AppUser.Email,
                CreatorName = survey.AppUser.Name,
                DateCreated = survey.DateCreated,
                SurveyQuestions = survey.SurveyQuestions?.Select(q => SurveyQuestionToDto(q)).ToList()
            };
        }
        public static SurveyQuestion SurveyQuestionFromUpdateDto(UpdateSurveyQuestionDto qDto, Survey survey)
        {
            return new SurveyQuestion
            {
                Text = qDto.Text,
                SurveyId = survey.Id,
                SurveyResponses = qDto.SurveyResponses.Select(r => new SurveyResponse
                {
                    Text = r.Text,
                    Survey = survey
                }).ToList()
            };
        }
        public static SurveyResponse SurveyResponseFromUpdateDto(UpdateSurveyResponseDto updateSurveyResponseDto, Survey survey, SurveyQuestion question)
        {
            return new SurveyResponse
            {
                Text = updateSurveyResponseDto.Text,
                SurveyId = survey.Id,
                SurveyQuestionId = question.Id
            };
        }
        public static UserSurveyDto UserSurveyToDto(UserSurvey survey)
        {
            var dto = new UserSurveyDto
            {
                SurveyName = survey.Survey.Name,
                SurveyUrlName = survey.Survey.UrlName,
                UserUrlName = survey.AppUser.UrlName,
                Responses = new List<UserSurveyResponseDto>()
            };
            if (survey.UserResponses != null)
            {
                foreach (var response in survey.UserResponses)
                {
                    dto.Responses.Add(new UserSurveyResponseDto
                    {
                        SurveyQuestion = response.SurveyQuestion?.Text,
                        SurveyResponse = response.SurveyResponse?.Text
                    });
                }
            }

            return dto;
        }
    }
}
