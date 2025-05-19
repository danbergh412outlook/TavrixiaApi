using PortfolioApp.Api.Services;

namespace PortfolioApp.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            services.AddScoped<ISurveyService, SurveyService>();
            services.AddScoped<IUserSurveyService, UserSurveyService>();
            services.AddScoped<IGoogleUserService, GoogleUserService>();
            services.AddScoped<IAppUserService, AppUserService>();

            return services;
        }
    }
}
