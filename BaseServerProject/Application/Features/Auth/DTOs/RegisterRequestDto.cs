using System.ComponentModel.DataAnnotations;

namespace BaseServerProject.Application.Features.Auth.DTOs;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [MaxLength(255, ErrorMessage = "Email must not exceed 255 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Username is required")]
    [MaxLength(100, ErrorMessage = "First name must not exceed 100 characters")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    [MaxLength(100, ErrorMessage = "Password must not exceed 100 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
        ErrorMessage = "Password must contain at least one uppercase, one lowercase, one number, and one special character")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    [Compare("Password", ErrorMessage = "Password and confirmation password do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "First name must not exceed 100 characters")]
    public string FirstName { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "Last name must not exceed 100 characters")]
    public string LastName { get; set; } = string.Empty;
}