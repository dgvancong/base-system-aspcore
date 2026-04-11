using MediatR;
using BaseServerProject.Application.Features.Auth.DTOs;

namespace BaseServerProject.Application.Features.Auth.Commands;

public class RefreshTokenCommand : IRequest<RefreshTokenResult>
{
    public string RefreshToken { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}

public class RefreshTokenResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public LoginResponseDto? Data { get; set; }
}