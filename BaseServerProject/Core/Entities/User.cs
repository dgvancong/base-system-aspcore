using BaseServerProject.Core.Enums;

namespace BaseServerProject.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsEmailConfirmed { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public UserStatus Status { get; set; }

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        // Business methods
        public void RecordSuccessfulLogin()
        {
            FailedLoginAttempts = 0;
            LastLoginAt = DateTime.UtcNow;
            Status = UserStatus.Active;
            LockoutEnd = null;
        }

        public void RecordFailedLogin()
        {
            FailedLoginAttempts++;

            if (FailedLoginAttempts >= 5)
            {
                LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                Status = UserStatus.Locked;
            }
        }

        public bool CanLogin()
        {
            if (Status != UserStatus.Active) return false;
            if (LockoutEnd.HasValue && LockoutEnd > DateTime.UtcNow) return false;
            return true;
        }

        public string GetFullName() => $"{FirstName} {LastName}".Trim();
    }
}
