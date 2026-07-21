using TelegramBotApp.Domain.Entities;
using TelegramBotApp.Domain.Enums;

namespace TelegramBotApp.Domain.Entities;

public class Notification
{
    public int Id { get; set; }
    public int TelegramUserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime? SentAt { get; set; }
    public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; } = 0;

    // Navigation Property
    public TelegramUser? TelegramUser { get; set; }
}