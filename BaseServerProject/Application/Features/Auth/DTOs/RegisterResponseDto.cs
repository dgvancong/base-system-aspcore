namespace BaseServerProject.Application.Features.Auth.DTOs;

public class RegisterResponseDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool RequiresEmailConfirmation { get; set; }
    public DateTime CreatedAt { get; set; }
}