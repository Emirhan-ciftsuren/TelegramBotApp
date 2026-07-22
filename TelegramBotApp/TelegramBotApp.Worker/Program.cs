using Microsoft.EntityFrameworkCore;
using TelegramBotApp.Application.Interfaces;
using TelegramBotApp.Infrastructure.Persistence;
using TelegramBotApp.Worker;

var builder = Host.CreateApplicationBuilder(args);

// DbContext Kaydý
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<AppDbContext>());

// Worker Service Kaydý
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();