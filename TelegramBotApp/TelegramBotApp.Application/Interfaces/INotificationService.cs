using TelegramBotApp.Application.DTOs;

namespace TelegramBotApp.Application.Interfaces;

public interface ITelegramUserService
{
    Task ServiceOrUpdateUserAsync(TelegramUserDto userDto);
    Task<long?> GetChatIdByTelegramUserIdAsync(long telegramUserId);
}