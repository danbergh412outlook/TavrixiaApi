namespace PortfolioApp.Api.DTOs
{
    public class SurveyDto
    {
        public int Id { get; set; }
        public string CreatorEmail { get; set; }
        public string CreatorName { get; set; }
        public string UrlName { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public List<SurveyQuestionDto> SurveyQuestions { get; set; }
    }
}
