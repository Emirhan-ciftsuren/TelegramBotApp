using TelegramBotApp.Domain.Entities;

namespace TelegramBotApp.Domain.Entities;

public class TelegramUser
{
    public int Id { get; set; }
    public long TelegramUserId { get; set; }
    public string? Username { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public long ChatId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    // Navigation Property
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<BotCommand> BotCommands { get; set; } = new List<BotCommand>();
}