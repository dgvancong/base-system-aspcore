namespace BaseServerProject.Application.Common.Interfaces;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    string GenerateRandomPassword(int length = 12);
}