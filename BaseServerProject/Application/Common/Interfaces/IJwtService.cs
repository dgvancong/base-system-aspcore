using System.Security.Claims;
using BaseServerProject.Core.Entities;

namespace BaseServerProject.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(int userId, string ipAddress);
    ClaimsPrincipal? ValidateToken(string token);
}