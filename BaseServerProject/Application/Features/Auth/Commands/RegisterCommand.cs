using MediatR;
using BaseServerProject.Application.Features.Auth.DTOs;

namespace BaseServerProject.Application.Features.Auth.Commands;

public class RegisterCommand : IRequest<RegisterResult>
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
}

public class RegisterResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public RegisterResponseDto? Data { get; set; }
    public List<string>? Errors { get; set; }
}