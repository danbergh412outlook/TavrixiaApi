using Microsoft.EntityFrameworkCore;
using PortfolioApp.Api.Data;
using PortfolioApp.Api.DTOs;
using PortfolioApp.Api.Helpers;
using PortfolioApp.Api.Models;

namespace PortfolioApp.Api.Services
{
    public class AppUserService : IAppUserService
    {
        private readonly IGoogleUserService _googleUser;
        private readonly AppDbContext _context;
        private AppUser _appUser;

        public AppUserService(IGoogleUserService googleUser, AppDbContext context)
        {
            _googleUser = googleUser;
            _context = context;
        }
        public async Task<AppUser?> FindUser(string email)
        {
            if (email == null)
            {
                return null;
            }
            return await _context.AppUsers.FirstOrDefaultAsync(a => a.Email.ToLower() == email.ToLower());
        }
        public async Task<HashSet<string>> GetExistingUserUrlNames(string baseSlug, int? userId)
        {
            var existingSlugs = await _context.AppUsers
                .Where(s => s.UrlName.StartsWith(baseSlug) && s.Id != userId)
                .Select(s => s.UrlName)
                .ToHashSetAsync();
            return existingSlugs;
        }
        public async Task<AppUser> GetOrCreateUser()
        {
            if (_appUser != null)
            {
                return _appUser;
            }
            else
            {
                var email = _googleUser.GetEmail();
                var name = _googleUser.GetFullName();

                if (string.IsNullOrWhiteSpace(email))
                    throw new ArgumentException("User email cannot be null, empty, or whitespace.", nameof(email));
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("User name cannot be null, empty, or whitespace.", nameof(name));

                _appUser = await FindUser(email);
                if (_appUser == null)
                {
                    var slug = await Utils.GenerateUniqueSlug(name, null, null, null, GetExistingUserUrlNames);
                    _appUser = new AppUser
                    {
                        Email = email,
                        Name = name,
                        UrlName = slug
                    };
                    _context.AppUsers.Add(_appUser);
                    await _context.SaveChangesAsync();
                }
            }

            return _appUser;
        }
    }
}
