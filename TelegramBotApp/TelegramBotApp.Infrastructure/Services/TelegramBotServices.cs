using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotApp.Application.DTOs;
using TelegramBotApp.Application.Interfaces;

namespace TelegramBotApp.Infrastructure.Services;

public class TelegramBotService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<TelegramBotService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public TelegramBotService(
        IConfiguration configuration,
        ILogger<TelegramBotService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;

        var token = configuration["BotConfiguration:BotToken"];
        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException("Telegram Bot Token appsettings.json içinde bulunamadı!");
        }

        _botClient = new TelegramBotClient(token);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _logger.LogInformation("Telegram Bot Dinleyicisi Başlatılıyor...");

        // v22 ile uyumlu StartReceiving imzası:
        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken
        );

        await Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message || message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;
        var telegramUserId = message.From?.Id ?? 0;

        _logger.LogInformation("Gelen Mesaj: '{MessageText}' | ChatId: {ChatId}", messageText, chatId);

        using var scope = _scopeFactory.CreateScope();
        var userService = scope.ServiceProvider.GetRequiredService<ITelegramUserService>();

        await userService.ServiceOrUpdateUserAsync(new TelegramUserDto
        {
            TelegramUserId = telegramUserId,
            ChatId = chatId,
            Username = message.From?.Username,
            FirstName = message.From?.FirstName ?? "Bilinmiyor",
            LastName = message.From?.LastName
        });

        if (messageText.StartsWith("/start"))
        {
            // v22: SendMessage kullanılıyor
            await botClient.SendMessage(
                chatId: chatId,
                text: $"Merhaba {message.From?.FirstName}! Botumuza hoş geldin. Sistemimize kaydın başarıyla yapıldı.",
                cancellationToken: cancellationToken
            );
        }
        else
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: $"Mesajınız alındı: \"{messageText}\"",
                cancellationToken: cancellationToken
            );
        }
    }

 
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Telegram Bot Polling hatası oluştu.");
        return Task.CompletedTask;
    }
}