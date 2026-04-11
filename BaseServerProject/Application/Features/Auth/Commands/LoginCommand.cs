using MediatR;
using BaseServerProject.Application.Features.Auth.DTOs;

namespace BaseServerProject.Application.Features.Auth.Commands;

public class LoginCommand : IRequest<LoginResult>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public bool RememberMe { get; set; } = false;
}

public class LoginResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public LoginResponseDto? Data { get; set; }
    public int? LockoutMinutesRemaining { get; set; }
}