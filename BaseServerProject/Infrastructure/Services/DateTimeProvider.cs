using BaseServerProject.Application.Common.Interfaces;

namespace BaseServerProject.Infrastructure.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime Now => DateTime.Now;

    public DateTime Today => DateTime.Today;
}