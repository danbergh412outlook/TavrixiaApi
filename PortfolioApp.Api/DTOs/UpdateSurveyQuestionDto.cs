namespace PortfolioApp.Api.DTOs
{
    public class UpdateSurveyQuestionDto
    {
        public int? Id { get; set; }
        public int? SurveyId { get; set; }
        public string Text { get; set; }
        public List<UpdateSurveyResponseDto> SurveyResponses { get; set; }
    }
}
