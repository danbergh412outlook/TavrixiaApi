namespace PortfolioApp.Api.DTOs
{
    public class SurveyQuestionDto
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string Text { get; set; }
        public List<SurveyResponseDto> SurveyResponses { get; set; }
    }
}
