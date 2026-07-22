using Microsoft.EntityFrameworkCore;
using TelegramBotApp.Application.DTOs;
using TelegramBotApp.Application.Interfaces;
using TelegramBotApp.Domain.Entities;
using TelegramBotApp.Infrastructure.Persistence;

namespace TelegramBotApp.Application.Services;

public class TelegramUserService : ITelegramUserService
{
    private readonly AppDbContext _context;

    public TelegramUserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task ServiceOrUpdateUserAsync(TelegramUserDto userDto)
    {
        // Kullanıcı veritabanımızda var mı kontrol ediyoruz
        var existingUser = await _context.TelegramUsers
            .FirstOrDefaultAsync(u => u.TelegramUserId == userDto.TelegramUserId);

        if (existingUser == null)
        {
            // Yoksa yeni kullanıcı oluşturup ekliyoruz
            var newUser = new TelegramUser
            {
                TelegramUserId = userDto.TelegramUserId,
                Username = userDto.Username,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                ChatId = userDto.ChatId,
                IsActive = true,
                RegisteredAt = DateTime.UtcNow
            };

            await _context.TelegramUsers.AddAsync(newUser);
        }
        else
        {
            // Varsa bilgilerini güncelliyoruz (kullanıcı adı veya adı değişmiş olabilir)
            existingUser.Username = userDto.Username;
            existingUser.FirstName = userDto.FirstName;
            existingUser.LastName = userDto.LastName;
            existingUser.ChatId = userDto.ChatId;
            existingUser.IsActive = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<long?> GetChatIdByTelegramUserIdAsync(long telegramUserId)
    {
        var user = await _context.TelegramUsers
            .FirstOrDefaultAsync(u => u.TelegramUserId == telegramUserId);

        return user?.ChatId;
    }
}