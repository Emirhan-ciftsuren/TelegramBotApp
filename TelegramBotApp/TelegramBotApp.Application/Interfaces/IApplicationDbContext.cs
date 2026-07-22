using Microsoft.EntityFrameworkCore;
using TelegramBotApp.Domain.Entities;

namespace TelegramBotApp.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TelegramUser> TelegramUsers { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<BotCommand> BotCommands { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}