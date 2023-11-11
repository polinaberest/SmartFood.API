using Moq;
using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartFood.Common.Configuration;
using SmartFood.Domain;
using SmartFood.Domain.Models;
using SmartFood.Infrastructure.Commands.Auth;
using SmartFood.Infrastructure.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using SmartFood.Common.Constants;

namespace SmartFood.Infrastructure.Services.Tests
{
    [TestClass]
    public class AuthServiceTests
    {
        // Mocks
        private ApplicationDbContext dbContextMock;
        private Mock<UserManager<User>> userManagerMock;
        private Mock<JwtSettings> jwtSettingsMock;
        private Mock<IJwtTokenService> jwtTokenServiceMock;

        [TestInitialize]
        public void Initialize()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "MovieListDatabase")
                .Options;
            dbContextMock = new ApplicationDbContext(options);
            userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            jwtSettingsMock = new Mock<JwtSettings>();
            jwtTokenServiceMock = new Mock<IJwtTokenService>();
        }

        [TestMethod]
        public async Task GenerateTokenForUserAsync_ShouldGenerateTokenAndRefreshToken()
        {
            // Arrange
            var authService = new AuthService(dbContextMock, userManagerMock.Object, jwtSettingsMock.Object, jwtTokenServiceMock.Object);
            var user = new User { Id = Guid.NewGuid(), Email = "test@example.com", Name = "Test User" };

            userManagerMock.Setup(x => x.GetClaimsAsync(user)).ReturnsAsync(new List<Claim>());
            jwtTokenServiceMock.Setup(x => x.CreateToken(It.IsAny<List<Claim>>())).Returns(() => new JwtSecurityToken());
            jwtTokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns("fakeRefreshToken");

            // Act
            var result = await authService.GenerateTokenForUserAsync(user);

            // Assert
            Assert.IsNotNull(result.Token);
            Assert.IsNotNull(result.RefreshToken);
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldCreateUserAndAssignRole()
        {
            // Arrange
            var authService = new AuthService(dbContextMock, userManagerMock.Object, jwtSettingsMock.Object, jwtTokenServiceMock.Object);
            var command = new RegisterCommand
            {
                Email = "test@example.com",
                Password = "password123",
                Name = "Test User",
                Role = UserRoles.Employee,
                OrganizationName = "TestOrg",
                Description = "Test Description"
            };

            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), command.Password)).ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(x => x.FindByEmailAsync(command.Email)).ReturnsAsync(new User());
            userManagerMock.Setup(x => x.AddClaimAsync(It.IsAny<User>(), It.IsAny<Claim>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await authService.RegisterUserAsync(command);

            // Assert
            Assert.IsNotNull(result);
        }

        //[TestMethod]
        //[ExpectedException(typeof(Exception))]
        //public async Task RegisterUserAsync_ShouldThrowExceptionOnFailure()
        //{
        //    // Arrange
        //    var authService = new AuthService(dbContextMock, userManagerMock.Object, jwtSettingsMock.Object, jwtTokenServiceMock.Object);
        //    var command = new RegisterCommand
        //    {
        //        Email = "test@example.com",
        //        Password = "password123",
        //        Name = "Test User",
        //        Role = "TestRole",
        //        OrganizationName = "TestOrg",
        //        Description = "Test Description"
        //    };

        //    userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), command.Password)).ReturnsAsync(IdentityResult.Failed());

        //    // Act & Assert
        //    await authService.RegisterUserAsync(command);
        //}

        [TestMethod]
        [ExpectedException(typeof(AuthenticationException))]
        public async Task RefreshToken_ShouldThrowExceptionOnInvalidToken()
        {
            // Arrange
            var authService = new AuthService(dbContextMock, userManagerMock.Object, jwtSettingsMock.Object, jwtTokenServiceMock.Object);
            var token = "fakeToken";
            var refreshToken = "fakeRefreshToken";
            jwtTokenServiceMock.Setup(x => x.GetPrincipalFromExpiredToken(token)).Returns((ClaimsPrincipal)null);

            // Act & Assert
            await authService.RefreshToken(token, refreshToken);
        }
    }
}
