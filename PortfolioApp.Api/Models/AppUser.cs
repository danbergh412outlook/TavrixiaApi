using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Api.Models
{
    [Index(nameof(UrlName), IsUnique = true)]
    [Index(nameof(Email), IsUnique = true)]
    public class AppUser
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string UrlName { get; set; }
        public ICollection<UserSurvey> UserSurveys { get; set; }
        public ICollection<Survey> Surveys { get; set; }
    }
}
