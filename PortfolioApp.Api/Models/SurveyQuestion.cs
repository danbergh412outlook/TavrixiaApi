using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Api.Models
{
    public class SurveyQuestion
    {
        public int Id { get; set; }
        [Required]
        public int SurveyId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Text { get; set; }

        public Survey Survey { get; set; }
        public ICollection<SurveyResponse> SurveyResponses { get; set; }
        public ICollection<UserResponse> UserResponses { get; set; }
    }
}
