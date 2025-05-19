using Microsoft.AspNetCore.Http;
using Moq;
using PortfolioApp.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiTestProject
{
    public class GoogleUserServiceTests
    {
        private static IHttpContextAccessor CreateHttpContextAccessor(bool isAuthenticated, IEnumerable<Claim>? claims = null)
        {
            var identity = new ClaimsIdentity(claims ?? [], isAuthenticated ? "TestAuthType" : null);
            var principal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext { User = principal };

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(a => a.HttpContext).Returns(context);

            return accessorMock.Object;
        }

        [Fact]
        public void GetEmail_ReturnsEmail_WhenAuthenticatedAndClaimExists()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.Email, "user@example.com") };
            var service = new GoogleUserService(CreateHttpContextAccessor(true, claims));

            // Act
            var result = service.GetEmail();

            // Assert
            Assert.Equal("user@example.com", result);
        }

        [Fact]
        public void GetEmail_ReturnsNull_WhenUnauthenticated()
        {
            // Arrange
            var service = new GoogleUserService(CreateHttpContextAccessor(false));

            // Act
            var result = service.GetEmail();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetEmail_ReturnsNull_WhenEmailClaimMissing()
        {
            // Arrange
            var claims = new[] { new Claim("name", "Jane Doe") };
            var service = new GoogleUserService(CreateHttpContextAccessor(true, claims));

            // Act
            var result = service.GetEmail();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetFullName_ReturnsName_WhenAuthenticatedAndClaimExists()
        {
            // Arrange
            var claims = new[] { new Claim("name", "Jane Doe") };
            var service = new GoogleUserService(CreateHttpContextAccessor(true, claims));

            // Act
            var result = service.GetFullName();

            // Assert
            Assert.Equal("Jane Doe", result);
        }

        [Fact]
        public void GetFullName_ReturnsNull_WhenUnauthenticated()
        {
            // Arrange
            var claims = new[] { new Claim("name", "Jane Doe") };
            var service = new GoogleUserService(CreateHttpContextAccessor(false, claims));

            // Act
            var result = service.GetFullName();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetFullName_ReturnsNull_WhenNameClaimMissing()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.Email, "user@example.com") };
            var service = new GoogleUserService(CreateHttpContextAccessor(true, claims));

            // Act
            var result = service.GetFullName();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetEmailAndName_CanBeReadTogether_WhenBothClaimsExist()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, "user@example.com"),
                new Claim("name", "Jane Doe")
            };
            var service = new GoogleUserService(CreateHttpContextAccessor(true, claims));

            // Act
            var email = service.GetEmail();
            var name = service.GetFullName();

            // Assert
            Assert.Equal("user@example.com", email);
            Assert.Equal("Jane Doe", name);
        }
    }
}
