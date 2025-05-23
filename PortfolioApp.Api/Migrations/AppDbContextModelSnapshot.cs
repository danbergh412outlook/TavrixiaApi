﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PortfolioApp.Api.Data;

#nullable disable

namespace PortfolioApp.Api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("PortfolioApp.Api.Models.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("UrlName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("UrlName")
                        .IsUnique();

                    b.ToTable("AppUsers");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.ResponseStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("ResponseStatuses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Not Started"
                        },
                        new
                        {
                            Id = 2,
                            Name = "In Progress"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Completed"
                        });
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.Survey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("AllowEmulation")
                        .HasColumnType("bit");

                    b.Property<int>("AppUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCompleted")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("SurveyStatusId")
                        .HasColumnType("int");

                    b.Property<string>("UrlName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.HasIndex("SurveyStatusId");

                    b.HasIndex("UrlName")
                        .IsUnique();

                    b.ToTable("Surveys");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.SurveyQuestion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("SurveyId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("SurveyId");

                    b.ToTable("SurveyQuestions");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.SurveyResponse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("SurveyId")
                        .HasColumnType("int");

                    b.Property<int>("SurveyQuestionId")
                        .HasColumnType("int");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("SurveyId");

                    b.HasIndex("SurveyQuestionId");

                    b.ToTable("SurveyResponses");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.SurveyStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("SurveyStatuses");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Draft"
                        },
                        new
                        {
                            Id = 2,
                            Name = "In Progress"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Completed"
                        });
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.UserResponse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("SurveyId")
                        .HasColumnType("int");

                    b.Property<int>("SurveyQuestionId")
                        .HasColumnType("int");

                    b.Property<int>("SurveyResponseId")
                        .HasColumnType("int");

                    b.Property<int>("UserSurveyId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SurveyId");

                    b.HasIndex("SurveyQuestionId");

                    b.HasIndex("SurveyResponseId");

                    b.HasIndex("UserSurveyId", "SurveyResponseId")
                        .IsUnique();

                    b.ToTable("UserSurveyResponses");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.UserSurvey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AppUserId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DateTaken")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsEmulation")
                        .HasColumnType("bit");

                    b.Property<int>("ResponseStatusId")
                        .HasColumnType("int");

                    b.Property<int>("SurveyId")
                        .HasColumnType("int");

                    b.Property<Guid?>("SurveyToken")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ResponseStatusId");

                    b.HasIndex("SurveyId");

                    b.HasIndex("AppUserId", "SurveyId")
                        .IsUnique();

                    b.ToTable("UserSurveys");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.Survey", b =>
                {
                    b.HasOne("PortfolioApp.Api.Models.AppUser", "AppUser")
                        .WithMany("Surveys")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PortfolioApp.Api.Models.SurveyStatus", "SurveyStatus")
                        .WithMany("Surveys")
                        .HasForeignKey("SurveyStatusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AppUser");

                    b.Navigation("SurveyStatus");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.SurveyQuestion", b =>
                {
                    b.HasOne("PortfolioApp.Api.Models.Survey", "Survey")
                        .WithMany("SurveyQuestions")
                        .HasForeignKey("SurveyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Survey");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.SurveyResponse", b =>
                {
                    b.HasOne("PortfolioApp.Api.Models.Survey", "Survey")
                        .WithMany()
                        .HasForeignKey("SurveyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("PortfolioApp.Api.Models.SurveyQuestion", "SurveyQuestion")
                        .WithMany("SurveyResponses")
                        .HasForeignKey("SurveyQuestionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Survey");

                    b.Navigation("SurveyQuestion");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.UserResponse", b =>
                {
                    b.HasOne("PortfolioApp.Api.Models.Survey", "Survey")
                        .WithMany()
                        .HasForeignKey("SurveyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PortfolioApp.Api.Models.SurveyQuestion", "SurveyQuestion")
                        .WithMany("UserResponses")
                        .HasForeignKey("SurveyQuestionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PortfolioApp.Api.Models.SurveyResponse", "SurveyResponse")
                        .WithMany("UserResponses")
                        .HasForeignKey("SurveyResponseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PortfolioApp.Api.Models.UserSurvey", "UserSurvey")
                        .WithMany("UserResponses")
                        .HasForeignKey("UserSurveyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Survey");

                    b.Navigation("SurveyQuestion");

                    b.Navigation("SurveyResponse");

                    b.Navigation("UserSurvey");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.UserSurvey", b =>
                {
                    b.HasOne("PortfolioApp.Api.Models.AppUser", "AppUser")
                        .WithMany("UserSurveys")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PortfolioApp.Api.Models.ResponseStatus", "ResponseStatus")
                        .WithMany("UserSurveys")
                        .HasForeignKey("ResponseStatusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("PortfolioApp.Api.Models.Survey", "Survey")
                        .WithMany("UserSurveys")
                        .HasForeignKey("SurveyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("AppUser");

                    b.Navigation("ResponseStatus");

                    b.Navigation("Survey");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.AppUser", b =>
                {
                    b.Navigation("Surveys");

                    b.Navigation("UserSurveys");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.ResponseStatus", b =>
                {
                    b.Navigation("UserSurveys");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.Survey", b =>
                {
                    b.Navigation("SurveyQuestions");

                    b.Navigation("UserSurveys");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.SurveyQuestion", b =>
                {
                    b.Navigation("SurveyResponses");

                    b.Navigation("UserResponses");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.SurveyResponse", b =>
                {
                    b.Navigation("UserResponses");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.SurveyStatus", b =>
                {
                    b.Navigation("Surveys");
                });

            modelBuilder.Entity("PortfolioApp.Api.Models.UserSurvey", b =>
                {
                    b.Navigation("UserResponses");
                });
#pragma warning restore 612, 618
        }
    }
}
