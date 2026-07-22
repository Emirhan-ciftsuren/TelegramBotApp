using TelegramBotApp.Application.DTOs;

namespace TelegramBotApp.Application.Interfaces;

public interface INotificationService
{
    Task CreateNotificationAsync(CreateNotificationDto dto);
    // İleride Worker Service'in arka planda bildirimleri çekmesi için kullanacağız:
    // Task<List<Notification>> GetPendingNotificationsAsync();
}