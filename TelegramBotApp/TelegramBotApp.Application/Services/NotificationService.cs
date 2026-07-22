using System;
using Microsoft.EntityFrameworkCore;
using TelegramBotApp.Domain.Enums;
using TelegramBotApp.Domain.Entities; 
using TelegramBotApp.Application.DTOs;
using TelegramBotApp.Application.Interfaces;


namespace TelegramBotApp.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IApplicationDbContext _context;

    public NotificationService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateNotificationAsync(CreateNotificationDto dto)
    {
        // ChatId üzerinden kullanıcıyı buluyoruz
        var user = await _context.TelegramUsers
            .FirstOrDefaultAsync(u => u.ChatId == dto.ChatId);

        if (user == null)
        {
            throw new Exception($"ChatId: {dto.ChatId} olan kullanıcı sistemde bulunamadı.");
        }

        var notification = new Notification
        {
            TelegramUserId = user.Id, // Foreign Key bağlantısı
            Message = dto.Message,
            Status = NotificationStatus.Pending, // Varsayılan olarak Beklemede
            RetryCount = 0
        };

        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }
}