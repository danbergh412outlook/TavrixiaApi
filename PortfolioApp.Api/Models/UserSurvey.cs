using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace PortfolioApp.Api.Models
{
    [Index(nameof(AppUserId), nameof(SurveyId), IsUnique = true)]
    public class UserSurvey
    {
        public int Id { get; set; }
        [Required]
        public int AppUserId { get; set; }
        [Required]
        public int SurveyId { get; set; }
        public DateTime? DateTaken { get; set; }
        public Guid? SurveyToken { get; set; }
        [Required]
        public bool IsEmulation { get; set; }
        [Required]
        public int ResponseStatusId { get; set; }
        public Survey Survey { get; set; }
        public AppUser AppUser { get; set; }
        public ICollection<UserResponse> UserResponses { get; set; }
        public ResponseStatus ResponseStatus { get; set; }
    }
}
