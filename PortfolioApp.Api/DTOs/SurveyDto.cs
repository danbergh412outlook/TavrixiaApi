namespace PortfolioApp.Api.DTOs
{
    public class SurveyDto
    {
        public int Id { get; set; }
        public string Creator { get; set; }
        public string UrlName { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
