namespace TelegramBotApp.Application.DTOs;

public class CreateNotificationDto
{
    public long ChatId { get; set; } // Telegram'daki hedef sohbet/kullanıcı ID'si
    public string Message { get; set; } = string.Empty;
}