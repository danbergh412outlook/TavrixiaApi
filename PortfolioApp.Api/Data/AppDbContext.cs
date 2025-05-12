using Microsoft.EntityFrameworkCore;
using PortfolioApp.Api.Models;

namespace PortfolioApp.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Survey> Surveys { get; set; }
        public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<UserSurvey> UserSurveys { get; set; }
        public DbSet<UserResponse> UserSurveyResponses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Survey>()
                .HasMany(s => s.SurveyQuestions)
                .WithOne(q => q.Survey)
                .HasForeignKey(q => q.SurveyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SurveyQuestion>()
                .HasMany(q => q.SurveyResponses)
                .WithOne(r => r.SurveyQuestion)
                .HasForeignKey(r => r.SurveyQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserSurvey>()
                .HasOne(us => us.Survey)
                .WithMany(s => s.UserSurveys)
                .HasForeignKey(us => us.SurveyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserResponse>()
                .HasOne(usr => usr.Survey)
                .WithMany()
                .HasForeignKey(usr => usr.SurveyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserResponse>()
                .HasOne(usr => usr.SurveyQuestion)
                .WithMany(q => q.UserResponses)
                .HasForeignKey(usr => usr.SurveyQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserResponse>()
                .HasOne(usr => usr.SurveyResponse)
                .WithMany(r => r.UserResponses)
                .HasForeignKey(usr => usr.SurveyResponseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
