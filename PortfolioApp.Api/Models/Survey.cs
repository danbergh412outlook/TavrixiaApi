using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Api.Models
{
    [Index(nameof(UrlName), IsUnique = true)]
    public class Survey
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Creator { get; set; }
        [Required]
        public DateTime DateCreated { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string UrlName { get; set; }

        public ICollection<SurveyQuestion> SurveyQuestions { get; set; }
        public ICollection<UserSurvey> UserSurveys { get; set; }
    }
}
