using Microsoft.EntityFrameworkCore;
using PortfolioApp.Api.Models;
using System.Data;

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
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<ResponseStatus> ResponseStatuses { get; set; }
        public DbSet<SurveyStatus> SurveyStatuses { get; set; }

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

            modelBuilder.Entity<AppUser>()
                .HasMany(s => s.UserSurveys)
                .WithOne(q => q.AppUser)
                .HasForeignKey(q => q.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppUser>()
                .HasMany(s => s.Surveys)
                .WithOne(q => q.AppUser)
                .HasForeignKey(q => q.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ResponseStatus>()
                .HasMany(s => s.UserSurveys)
                .WithOne(q => q.ResponseStatus)
                .HasForeignKey(q => q.ResponseStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SurveyStatus>()
                .HasMany(s => s.Surveys)
                .WithOne(q => q.SurveyStatus)
                .HasForeignKey(q => q.SurveyStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SurveyStatus>().HasData(
                new SurveyStatus { Id = 1, Name = "Draft" },
                new SurveyStatus { Id = 2, Name = "In Progress" },
                new SurveyStatus { Id = 3, Name = "Completed" }
            );

            modelBuilder.Entity<ResponseStatus>().HasData(
                new ResponseStatus { Id = 1, Name = "Not Started" },
                new ResponseStatus { Id = 2, Name = "In Progress" },
                new ResponseStatus { Id = 3, Name = "Completed" }
            );
        }
    }
}
