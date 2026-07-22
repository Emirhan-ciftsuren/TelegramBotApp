using Microsoft.EntityFrameworkCore;
using TelegramBotApp.Application.Interfaces;
using TelegramBotApp.Domain.Entities; 

namespace TelegramBotApp.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<TelegramUser> TelegramUsers => Set<TelegramUser>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<BotCommand> BotCommands => Set<BotCommand>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}