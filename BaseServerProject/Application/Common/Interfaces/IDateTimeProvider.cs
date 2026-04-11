namespace BaseServerProject.Application.Common.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
    DateTime Today { get; }
}