using TelegramBotApp.Domain.Entities;

namespace TelegramBotApp.Domain.Entities;

public class BotCommand
{
    public int Id { get; set; }
    public int TelegramUserId { get; set; }
    public string Command { get; set; } = string.Empty; // örn: /start, /help
    public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
    public bool Processed { get; set; } = false;

    // Navigation Property
    public TelegramUser? TelegramUser { get; set; }
}