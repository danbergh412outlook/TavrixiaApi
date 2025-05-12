using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Models;

namespace PortfolioApp.Api.Helpers
{
    public class Mappers
    {
        public static SurveyDto SurveyDtoToSurvey(Survey surveyDto)
        {
            return new SurveyDto
            {
                Id = surveyDto.Id,
                Name = surveyDto.Name,
                Creator = surveyDto.Creator,
                DateCreated = surveyDto.DateCreated,
                UrlName = surveyDto.UrlName
            };
        }
        public static SurveyQuestionDto SurveyQuestionToSurveyQuestionDto(SurveyQuestion question)
        {
            return new SurveyQuestionDto
            {
                Text = question.Text,
                Id = question.Id,
                SurveyId = question.SurveyId,
                SurveyResponses = question.SurveyResponses.Select(r => SurveyResponseToSurveyResponseDto(r)).ToList()
            };
        }
        public static SurveyResponseDto SurveyResponseToSurveyResponseDto(SurveyResponse question)
        {
            return new SurveyResponseDto
            {
                Text = question.Text,
                Id = question.Id,
                SurveyId = question.SurveyId,
                SurveyQuestionId = question.SurveyQuestionId
            };
        }
        public static SurveyDto SurveyToSurveyDto(Survey survey)
        {
            return new SurveyDto
            {
                Id = survey.Id,
                Name = survey.Name,
                UrlName = survey.UrlName,
                Creator = survey.Creator,
                DateCreated = survey.DateCreated
            };
        }
        public static SurveyDetailsDto SurveyToSurveyDetailsDto(Survey survey)
        {
            return new SurveyDetailsDto
            {
                Id = survey.Id,
                Name = survey.Name,
                UrlName = survey.UrlName,
                Creator = survey.Creator,
                DateCreated = survey.DateCreated,
                SurveyQuestions = survey.SurveyQuestions.Select(q => Mappers.SurveyQuestionToSurveyQuestionDto(q)).ToList()
            };
        }
    }
}
