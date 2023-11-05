using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartFood.Common.Configuration;
using SmartFood.Infrastructure.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SmartFood.Infrastructure.Tests
{
    [TestClass]
    public class JwtTokenServiceTests
    {
        private JwtTokenService jwtTokenService;

        [TestInitialize]
        public void Initialize()
        {
            var jwtSettings = new JwtSettings
            {
                Secret = "your_secret_key_here",
                ValidIssuer = "issuer",
                ValidAudience = "audience",
                TokenValidityInMinutes = 60 // Modify this value if needed
            };

            jwtTokenService = new JwtTokenService(jwtSettings);
        }

        [TestMethod]
        public void CreateToken_ValidClaims_ReturnsJwtSecurityToken()
        {
            // Arrange
            var authClaims = new[]
            {
                new Claim(ClaimTypes.Name, "user123"),
                new Claim(ClaimTypes.Role, "admin")
            };

            // Act
            var token = jwtTokenService.CreateToken(authClaims);

            // Assert
            Assert.IsNotNull(token);
            Assert.IsInstanceOfType(token, typeof(JwtSecurityToken));
        }

        [TestMethod]
        public void GenerateRefreshToken_GeneratesNonEmptyString()
        {
            // Act
            var refreshToken = jwtTokenService.GenerateRefreshToken();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(refreshToken));


            

        }

        [TestMethod]
        public void GenerateRefreshToken_ReturnsUniqueTokens()
        {
            // Act
            var refreshToken1 = jwtTokenService.GenerateRefreshToken();
            var refreshToken2 = jwtTokenService.GenerateRefreshToken();

            // Assert
            Assert.AreNotEqual(refreshToken1, refreshToken2);
        }
    }
}
