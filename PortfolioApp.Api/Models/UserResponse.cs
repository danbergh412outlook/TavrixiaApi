using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Api.Models
{
    [Index(nameof(UserSurveyId), nameof(SurveyResponseId), IsUnique = true)]
    public class UserResponse
    {
        public int Id { get; set; }
        [Required]
        public int UserSurveyId { get; set; }
        [Required]
        public int SurveyId { get; set; }
        [Required]
        public int SurveyQuestionId { get; set; }
        [Required]
        public int SurveyResponseId { get; set; }

        public UserSurvey UserSurvey { get; set; }
        public Survey Survey { get; set; }
        public SurveyQuestion SurveyQuestion { get; set; }
        public SurveyResponse SurveyResponse { get; set; }
    }
}
