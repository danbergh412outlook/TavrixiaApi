namespace PortfolioApp.Api.DTOs
{
    public class UserSurveyDto
    {
        public string UserUrlName { get; set; }
        public string SurveyName { get; set; }
        public string SurveyUrlName { get; set; }
        public List<UserSurveyResponseDto> Responses { get; set; }
    }
}
