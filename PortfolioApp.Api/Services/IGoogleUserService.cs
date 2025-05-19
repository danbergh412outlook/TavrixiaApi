namespace PortfolioApp.Api.Services
{
    public interface IGoogleUserService
    {
        string? GetEmail();
        string? GetFullName();
    }
}