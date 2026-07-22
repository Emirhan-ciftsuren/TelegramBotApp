namespace TelegramBotApp.Application.DTOs;

public class TelegramUserDto
{
    public long TelegramUserId { get; set; }
    public string? Username { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public long ChatId { get; set; }
}