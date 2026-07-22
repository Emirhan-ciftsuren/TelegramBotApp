using Microsoft.EntityFrameworkCore;
using TelegramBotApp.Application.Interfaces;
using TelegramBotApp.Application.Services;
using TelegramBotApp.Infrastructure.Persistence;
using TelegramBotApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný (DbContext) Kaydý
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("TelegramBotApp.Infrastructure")));

// 2. Application Katmaný Servis Kayýtlarý (EKLENEN KISIM)
builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());
builder.Services.AddScoped<ITelegramUserService, TelegramUserService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// 3. Controller ve Swagger Kayýtlarý
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Telegram Bot Arka Plan Servisi
builder.Services.AddHostedService<TelegramBotService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();