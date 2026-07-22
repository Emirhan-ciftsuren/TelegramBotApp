using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using TelegramBotApp.Application.Interfaces;
using TelegramBotApp.Domain.Enums;

namespace TelegramBotApp.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ITelegramBotClient _botClient;

    public Worker(
        ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

        var token = configuration["BotConfiguration:BotToken"];
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException("Worker için Telegram Bot Token bulunamadý!");
        }

        _botClient = new TelegramBotClient(token);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification Worker Servisi Baþlatýldý.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingNotificationsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kuyruktaki bildirimler iþlenirken hata oluþtu.");
            }

            // 5 saniyede bir kuyruðu kontrol et
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task ProcessPendingNotificationsAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

        // 1. Bekleyen bildirimleri çek (EF Core / LINQ)
        var pendingNotifications = await context.Notifications
            .Where(n => n.Status == NotificationStatus.Pending)
            .OrderBy(n => n.Id)
            .Take(10)
            .ToListAsync(cancellationToken);

        if (!pendingNotifications.Any())
            return;

        _logger.LogInformation("{Count} adet bekleyen bildirim bulundu, iþleniyor...", pendingNotifications.Count);

        // 2. Tüm kullanýcýlarý hafýzaya al veya eþleþtir
        var userList = await context.TelegramUsers.ToListAsync(cancellationToken);

        foreach (var notification in pendingNotifications)
        {
            try
            {
                // Hem veritabaný Id'si (1) hem de TelegramUserId (6453838724) ile esnek arama yapýyoruz
                var user = userList.FirstOrDefault(u =>
                    u.Id == notification.TelegramUserId ||
                    u.TelegramUserId == notification.TelegramUserId);

                if (user == null || user.ChatId == 0)
                {
                    _logger.LogWarning("Notification ID {Id} için geçerli ChatId/Kullanýcý bulunamadý. Aranan ID: {TId}",
                        notification.Id, notification.TelegramUserId);

                    notification.Status = NotificationStatus.Failed;
                    notification.ErrorMessage = "Kullanýcý veya ChatId bulunamadý.";
                    continue;
                }

                // Telegram'a mesaj gönder
                await _botClient.SendMessage(
                    chatId: user.ChatId,
                    text: notification.Message,
                    cancellationToken: cancellationToken
                );

                notification.Status = NotificationStatus.Sent;
                notification.SentAt = DateTime.UtcNow;
                _logger.LogInformation("Notification ID {Id} BAÞARIYLA GÖNDERÝLDÝ -> ChatId: {ChatId}", notification.Id, user.ChatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Notification ID {Id} gönderilirken hata oluþtu.", notification.Id);

                notification.RetryCount++;
                notification.ErrorMessage = ex.Message;
                if (notification.RetryCount >= 3)
                {
                    notification.Status = NotificationStatus.Failed;
                }
            }
        }

        // Deðiþiklikleri veritabanýna kaydet
        await context.SaveChangesAsync(cancellationToken);
    }
}