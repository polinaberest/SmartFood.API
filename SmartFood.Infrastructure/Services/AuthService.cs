using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartFood.Common.Configuration;
using SmartFood.Common.Constants;
using SmartFood.Domain;
using SmartFood.Domain.Models;
using SmartFood.Infrastructure.Commands.Auth;
using SmartFood.Infrastructure.Constants;
using SmartFood.Infrastructure.Extensions;
using SmartFood.Infrastructure.Services.Interfaces;

namespace SmartFood.Infrastructure.Services;

public interface IAuthService
{
    Task<(string Token, string RefreshToken)> GenerateTokenForUserAsync(User user);

    Task<User> RegisterUserAsync(RegisterCommand command);

    Task<(string Token, string RefreshToken)> RefreshToken(string token, string refreshToken);
}

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext dbContext;
    private readonly UserManager<User> userManager;
    private readonly JwtSettings jwtSettings;
    private readonly IJwtTokenService jwtTokenService;

    public AuthService(ApplicationDbContext dbContext, UserManager<User> userManager, JwtSettings jwtSettings, IJwtTokenService jwtTokenService)
    {
        this.dbContext = dbContext;
        this.userManager = userManager;
        this.jwtSettings = jwtSettings;
        this.jwtTokenService = jwtTokenService;
    }

    public async Task<(string Token, string RefreshToken)> GenerateTokenForUserAsync(User user)
    {
        var authClaims = new List<Claim>
        {
            new(JwtClaims.Sub, user.Id.ToString()),
            new(JwtClaims.Email, user.Email!),
            new(JwtClaims.RegisterDate, user.RegisterDate.ToString("O")),
            new(JwtClaims.Name, user.Name)
        };

        var userClaims = await userManager.GetClaimsAsync(user);
        var roleClaims = userClaims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();

        authClaims.Add(new Claim(JwtClaims.Roles, string.Join<string>(",", roleClaims)));

        var token = jwtTokenService.CreateToken(authClaims);
        var refreshToken = jwtTokenService.GenerateRefreshToken();


        var serializedToken = token.SerializeToString();

        var issuedToken = new IssuedToken
        {
            User = user,
            Token = serializedToken,
            RefreshToken = refreshToken,
            RefreshTokenExpirationTime = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenValidityInDays)
        };

        dbContext.IssuedTokens.Add(issuedToken);
        await dbContext.SaveChangesAsync();

        return (serializedToken, refreshToken);
    }

    public async Task<User> RegisterUserAsync(RegisterCommand command)
    {
        var user = new User()
        {
            Email = command.Email,
            UserName = command.Email,
            Name = command.Name
        };

        var result = await userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            throw new Exception($"Unexpected error during user registration: {string.Join(", ", result.Errors)}.");
        }

        var createdUser = await userManager.FindByEmailAsync(user.Email);
        if (createdUser is null)
        {
            throw new Exception("Registered user not found.");
        }

        try
        {
            var claimAssignmentResult = await userManager.AddClaimAsync(createdUser, new Claim(ClaimTypes.Role, command.Role.ToLower()));

            if (!claimAssignmentResult.Succeeded)
            {
                throw new ArgumentException($"Unexpected error during role claim assignment: {string.Join(", ", claimAssignmentResult.Errors)}.", nameof(command.Role));
            }

            // May be factory or some dictionary with strategies. For now lets keep it simple.
            switch (command.Role.ToLower())
            {
                case UserRoles.OrganizationManager:
                    var organization = new Organization
                    {
                        ManagerId = createdUser.Id,
                        Name = command.OrganizationName,
                        Description = command.Description
                    };
                    dbContext.Organizations.Add(organization);
                    break;
                case UserRoles.Supplier:
                    var supplier = new Supplier
                    {
                        ManagerId = createdUser.Id,
                        Name = command.OrganizationName,
                        Description = command.Description
                    };
                    dbContext.Suppliers.Add(supplier);
                    break;
                default:
                    throw new ArgumentException($"Registration for role {command.Role} is not supported.", nameof(command.Role));
            }

            await dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {
            // Rollback user manages changes if transaction fails.
            await userManager.DeleteAsync(createdUser);
            throw;
        }

        return createdUser;
    }

    public async Task<(string Token, string RefreshToken)> RefreshToken(string token, string refreshToken)
    {
        var principal = this.jwtTokenService.GetPrincipalFromExpiredToken(token);
        if (principal == null)
        {
            throw new ArgumentException("Invalid access token or refresh token");
        }

        var userId = Guid.Parse(principal.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value);

        var user = await dbContext.Users
            .Include(c => c.IssuedTokens)
            .Where(u => u.Id == userId &&
                        u.IssuedTokens.Any(t =>
                            t.RefreshToken == refreshToken && DateTime.UtcNow <= t.RefreshTokenExpirationTime))
            .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new ArgumentException("Invalid access token or refresh token");
        }

        var currentToken = user.IssuedTokens.First(c => c.RefreshToken == refreshToken);
        dbContext.IssuedTokens.Remove(currentToken);

        return await GenerateTokenForUserAsync(user);
    }
}