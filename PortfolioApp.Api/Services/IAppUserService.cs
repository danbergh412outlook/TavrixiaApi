using PortfolioApp.Api.Models;

namespace PortfolioApp.Api.Services
{
    public interface IAppUserService
    {
        Task<AppUser?> FindUser(string email);
        Task<HashSet<string>> GetExistingUserUrlNames(string baseSlug, int? userId);
        Task<AppUser> GetOrCreateUser();
    }
}