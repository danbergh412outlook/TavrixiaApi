namespace PortfolioApp.Api.DTOs
{
    public class UpdateSurveyDto
    {
        public int? Id { get; set; }
        public string Creator { get; set; }
        public string Name { get; set; }
        public List<UpdateSurveyQuestionDto> SurveyQuestions { get; set; }
    }
}
