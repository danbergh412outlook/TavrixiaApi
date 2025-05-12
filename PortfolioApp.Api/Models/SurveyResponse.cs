using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Api.Models
{
    public class SurveyResponse
    {
        public int Id { get; set; }
        [Required]
        public int SurveyId { get; set; }
        [Required]
        public int SurveyQuestionId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Text { get; set; }

        public Survey Survey { get; set; }
        public SurveyQuestion SurveyQuestion { get; set; }
        public ICollection<UserResponse> UserResponses { get; set; }
    }
}
