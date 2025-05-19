using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Api.Models
{
    [Index(nameof(UrlName), IsUnique = true)]
    public class Survey
    {
        public int Id { get; set; }
        [Required]
        public int AppUserId { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        public DateTime DateCompleted { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string UrlName { get; set; }
        [Required]
        public bool AllowEmulation { get; set; }
        [Required]
        public int SurveyStatusId { get; set; }

        public ICollection<SurveyQuestion> SurveyQuestions { get; set; }
        public ICollection<UserSurvey> UserSurveys { get; set; }
        public AppUser AppUser { get; set; }
        public SurveyStatus SurveyStatus { get; set; }
    }
}
