namespace PortfolioApp.Api.DTOs
{
    public class SurveyResponseDto
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int SurveyQuestionId { get; set; }
        public string Text { get; set; }
    }
}
